using System;
using AlreadyOne.Data;
using UnityEngine;

namespace AlreadyOne.Game
{
    [DisallowMultipleComponent]
    public sealed class GameManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private MazeRenderer _mazeRenderer;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private FlipManager _flipManager;

        [Header("Stage")]
        [SerializeField] [Min(1)] private int _startingStageNumber = 1;

        private StageData _stageData;
        private bool[] _collectedKeys = Array.Empty<bool>();
        private bool[] _openedDoors = Array.Empty<bool>();
        private bool _hasLoadedStage;
        private bool _isStageCleared;
        private int _currentStageNumber;

        public event Action OnStageCleared;

        private void OnEnable()
        {
            if (_playerController != null)
            {
                _playerController.OnMoved += HandlePlayerMoved;
            }

            if (_flipManager != null)
            {
                _flipManager.OnFlipped += HandleFlipped;
            }
        }

        private void Start()
        {
            if (_hasLoadedStage || !HasRequiredReferences())
            {
                return;
            }

            LoadStage(Mathf.Max(1, _startingStageNumber));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartStage();
            }
        }

        private void OnDisable()
        {
            if (_playerController != null)
            {
                _playerController.OnMoved -= HandlePlayerMoved;
            }

            if (_flipManager != null)
            {
                _flipManager.OnFlipped -= HandleFlipped;
            }
        }

        public void LoadStage(int stageNumber)
        {
            if (!HasRequiredReferences())
            {
                return;
            }

            StageData loadedStage = StageLoader.LoadStage(stageNumber);
            if (loadedStage == null)
            {
                Debug.LogError($"Failed to load stage {stageNumber}.");
                return;
            }

            _stageData = loadedStage;
            _currentStageNumber = stageNumber;
            _hasLoadedStage = true;
            _isStageCleared = false;

            InitializeInteractionState();

            _mazeRenderer.Initialize(_stageData);
            _mazeRenderer.SetInteractionState(_collectedKeys, _openedDoors);

            _playerController.SetBoardTransform(_mazeRenderer.transform);
            _playerController.SetStageData(_stageData, true);
            _playerController.SetDoorStates(_openedDoors);
            _playerController.SetPosition(_stageData.StartX, _stageData.StartY);

            _flipManager.SetPlayerController(_playerController);
            _flipManager.SetStageData(_stageData);
            _flipManager.SetDoorStates(_openedDoors);
            _flipManager.Reset();

            ApplyFaceState(_flipManager.IsFront);
            EvaluateCurrentTile(_playerController.GridX, _playerController.GridY);
        }

        public void RestartStage()
        {
            int stageNumber = _currentStageNumber > 0 ? _currentStageNumber : Mathf.Max(1, _startingStageNumber);
            LoadStage(stageNumber);
        }

        private void HandlePlayerMoved(int x, int y)
        {
            EvaluateCurrentTile(x, y);
        }

        private void HandleFlipped(bool isFront)
        {
            ApplyFaceState(isFront);
            EvaluateCurrentTile(_playerController.GridX, _playerController.GridY);
        }

        private void ApplyFaceState(bool isFront)
        {
            _mazeRenderer.SetFace(isFront);
            _playerController.SetStageData(_stageData, isFront);
        }

        private void EvaluateCurrentTile(int x, int y)
        {
            if (_stageData == null)
            {
                return;
            }

            if (!_flipManager.IsFront && TryCollectKeyAt(x, y))
            {
                SyncInteractionState();
            }

            if (_isStageCleared)
            {
                return;
            }

            if (x == _stageData.GoalX && y == _stageData.GoalY)
            {
                _isStageCleared = true;
                OnStageCleared?.Invoke();
            }
        }

        private bool TryCollectKeyAt(int x, int y)
        {
            PositionData[] keys = _stageData.Keys;
            if (keys == null)
            {
                return false;
            }

            bool stateChanged = false;

            for (int i = 0; i < keys.Length; i++)
            {
                PositionData key = keys[i];
                if (key == null || key.X != x || key.Y != y || _collectedKeys[i])
                {
                    continue;
                }

                _collectedKeys[i] = true;

                // Keys and doors are paired by array index.
                if (i < _openedDoors.Length)
                {
                    _openedDoors[i] = true;
                }

                stateChanged = true;
            }

            return stateChanged;
        }

        private void InitializeInteractionState()
        {
            int keyCount = _stageData.Keys?.Length ?? 0;
            int doorCount = _stageData.Doors?.Length ?? 0;

            _collectedKeys = new bool[keyCount];
            _openedDoors = new bool[doorCount];
        }

        private void SyncInteractionState()
        {
            _mazeRenderer.SetInteractionState(_collectedKeys, _openedDoors);
            _playerController.SetDoorStates(_openedDoors);
            _flipManager.SetDoorStates(_openedDoors);
        }

        private bool HasRequiredReferences()
        {
            if (_mazeRenderer != null && _playerController != null && _flipManager != null)
            {
                return true;
            }

            Debug.LogError($"{nameof(GameManager)} is missing one or more scene references.");
            return false;
        }
    }
}
