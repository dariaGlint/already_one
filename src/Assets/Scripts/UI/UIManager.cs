using System;
using AlreadyOne.Game;
using UnityEngine;

namespace AlreadyOne.UI
{
    [DisallowMultipleComponent]
    public sealed class UIManager : MonoBehaviour
    {
        private enum UIFlowState
        {
            Title,
            Playing,
            StageClear,
            GameClear
        }

        [Header("Managers")]
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private StageProgressManager _stageProgressManager;
        [SerializeField] private FlipManager _flipManager;

        [Header("UI")]
        [SerializeField] private TitleScreenUI _titleScreenUI;
        [SerializeField] private StageClearUI _stageClearUI;
        [SerializeField] private GameClearUI _gameClearUI;
        [SerializeField] private FlipIndicatorUI _flipIndicatorUI;

        private PlayerController _playerController;
        private UIFlowState _flowState;
        private int _availableStageCount;

        private void Awake()
        {
            CacheReferences();
            _availableStageCount = CountAvailableStages();
            SetGameplayActive(false);
            _flowState = UIFlowState.Title;
        }

        private void Start()
        {
            ShowTitleState();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void HandleGameStarted()
        {
            if (_gameManager == null || _stageProgressManager == null)
            {
                Debug.LogError($"{nameof(UIManager)} is missing required manager references.");
                return;
            }

            _stageProgressManager.ResetProgress();
            SetGameplayActive(true);

            int stageNumber = Mathf.Max(1, _stageProgressManager.CurrentStageNumber);
            _gameManager.LoadStage(stageNumber);

            ShowPlayingState();
        }

        private void HandleStageCleared()
        {
            if (_flowState != UIFlowState.Playing || _stageProgressManager == null)
            {
                return;
            }

            SetGameplayActive(false);

            int clearedStageNumber = Mathf.Max(1, _stageProgressManager.CurrentStageNumber);
            if (IsFinalStage(clearedStageNumber))
            {
                ShowGameClearState();
                return;
            }

            ShowStageClearState(clearedStageNumber);
        }

        private void HandleNextStageRequested()
        {
            if (_flowState != UIFlowState.StageClear || _stageProgressManager == null || _gameManager == null)
            {
                return;
            }

            _stageProgressManager.AdvanceToNextStage();
            if (_stageProgressManager.IsAllCleared)
            {
                return;
            }

            SetGameplayActive(true);
            _gameManager.LoadStage(_stageProgressManager.CurrentStageNumber);
            ShowPlayingState();
        }

        private void HandleAllStagesCleared()
        {
            SetGameplayActive(false);
            ShowGameClearState();
        }

        private void HandleReturnToTitleRequested()
        {
            _stageProgressManager?.ResetProgress();
            SetGameplayActive(false);
            ShowTitleState();
        }

        private void HandleFlipped(bool isFront)
        {
            _flipIndicatorUI?.SetFace(isFront);
        }

        private void CacheReferences()
        {
            if (_gameManager == null)
            {
                TryGetComponent(out _gameManager);
                _gameManager = _gameManager ?? UnityEngine.Object.FindFirstObjectByType<GameManager>();
            }

            if (_stageProgressManager == null)
            {
                TryGetComponent(out _stageProgressManager);
                _stageProgressManager = _stageProgressManager ?? UnityEngine.Object.FindFirstObjectByType<StageProgressManager>();
            }

            if (_flipManager == null)
            {
                TryGetComponent(out _flipManager);
                _flipManager = _flipManager ?? UnityEngine.Object.FindFirstObjectByType<FlipManager>();
            }

            if (_titleScreenUI == null)
            {
                TryGetComponent(out _titleScreenUI);
                _titleScreenUI = _titleScreenUI ?? UnityEngine.Object.FindFirstObjectByType<TitleScreenUI>();
            }

            if (_stageClearUI == null)
            {
                TryGetComponent(out _stageClearUI);
                _stageClearUI = _stageClearUI ?? UnityEngine.Object.FindFirstObjectByType<StageClearUI>();
            }

            if (_gameClearUI == null)
            {
                TryGetComponent(out _gameClearUI);
                _gameClearUI = _gameClearUI ?? UnityEngine.Object.FindFirstObjectByType<GameClearUI>();
            }

            if (_flipIndicatorUI == null)
            {
                TryGetComponent(out _flipIndicatorUI);
                _flipIndicatorUI = _flipIndicatorUI ?? UnityEngine.Object.FindFirstObjectByType<FlipIndicatorUI>();
            }

            _playerController = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
        }

        private void SubscribeEvents()
        {
            if (_titleScreenUI != null)
            {
                _titleScreenUI.OnGameStarted += HandleGameStarted;
            }

            if (_gameManager != null)
            {
                _gameManager.OnStageCleared += HandleStageCleared;
            }

            if (_stageProgressManager != null)
            {
                _stageProgressManager.OnAllStagesCleared += HandleAllStagesCleared;
            }

            if (_stageClearUI != null)
            {
                _stageClearUI.OnNextStageRequested += HandleNextStageRequested;
            }

            if (_gameClearUI != null)
            {
                _gameClearUI.OnReturnToTitleRequested += HandleReturnToTitleRequested;
            }

            if (_flipManager != null)
            {
                _flipManager.OnFlipped += HandleFlipped;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_titleScreenUI != null)
            {
                _titleScreenUI.OnGameStarted -= HandleGameStarted;
            }

            if (_gameManager != null)
            {
                _gameManager.OnStageCleared -= HandleStageCleared;
            }

            if (_stageProgressManager != null)
            {
                _stageProgressManager.OnAllStagesCleared -= HandleAllStagesCleared;
            }

            if (_stageClearUI != null)
            {
                _stageClearUI.OnNextStageRequested -= HandleNextStageRequested;
            }

            if (_gameClearUI != null)
            {
                _gameClearUI.OnReturnToTitleRequested -= HandleReturnToTitleRequested;
            }

            if (_flipManager != null)
            {
                _flipManager.OnFlipped -= HandleFlipped;
            }
        }

        private void ShowTitleState()
        {
            _flowState = UIFlowState.Title;
            _titleScreenUI?.Show();
            _stageClearUI?.Hide();
            _gameClearUI?.Hide();
            _flipIndicatorUI?.Hide();
        }

        private void ShowPlayingState()
        {
            _flowState = UIFlowState.Playing;
            _titleScreenUI?.Hide();
            _stageClearUI?.Hide();
            _gameClearUI?.Hide();

            if (_flipIndicatorUI != null)
            {
                _flipIndicatorUI.SetFace(_flipManager == null || _flipManager.IsFront);
                _flipIndicatorUI.Show();
            }
        }

        private void ShowStageClearState(int clearedStageNumber)
        {
            _flowState = UIFlowState.StageClear;
            _titleScreenUI?.Hide();
            _gameClearUI?.Hide();
            _flipIndicatorUI?.Hide();
            _stageClearUI?.Show(clearedStageNumber);
        }

        private void ShowGameClearState()
        {
            _flowState = UIFlowState.GameClear;
            _titleScreenUI?.Hide();
            _stageClearUI?.Hide();
            _flipIndicatorUI?.Hide();
            _gameClearUI?.Show();
        }

        private void SetGameplayActive(bool isActive)
        {
            if (_gameManager != null)
            {
                _gameManager.enabled = isActive;
            }

            if (_flipManager != null)
            {
                _flipManager.enabled = isActive;
            }

            if (_playerController != null)
            {
                _playerController.enabled = isActive;
            }
        }

        private int CountAvailableStages()
        {
            TextAsset[] stageAssets = Resources.LoadAll<TextAsset>("Stages");
            if (stageAssets == null || stageAssets.Length == 0)
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < stageAssets.Length; i++)
            {
                TextAsset stageAsset = stageAssets[i];
                if (stageAsset != null && stageAsset.name.StartsWith("stage_", StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsFinalStage(int stageNumber)
        {
            return _availableStageCount > 0 && stageNumber >= _availableStageCount;
        }
    }
}
