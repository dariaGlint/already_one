using System;
using AlreadyOne.Data;
using UnityEngine;

namespace AlreadyOne.Game
{
    [DisallowMultipleComponent]
    public sealed class FlipManager : MonoBehaviour
    {
        private StageData _stageData;
        private PlayerController _playerController;
        private bool[] _openedDoors = Array.Empty<bool>();

        public event Action<bool> OnFlipped;
        public event Action OnFlipFailed;

        public bool IsFront { get; private set; } = true;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                return;
            }

            if (_stageData == null || _playerController == null)
            {
                OnFlipFailed?.Invoke();
                return;
            }

            TryFlip(_playerController.GridX, _playerController.GridY, _stageData);
        }

        public bool TryFlip(int playerX, int playerY, StageData stageData)
        {
            if (stageData == null)
            {
                OnFlipFailed?.Invoke();
                return false;
            }

            bool nextIsFront = !IsFront;
            if (!stageData.IsWalkable(playerX, playerY, nextIsFront) || IsClosedDoor(playerX, playerY, nextIsFront, stageData))
            {
                OnFlipFailed?.Invoke();
                return false;
            }

            IsFront = nextIsFront;
            OnFlipped?.Invoke(IsFront);
            return true;
        }

        public void Reset()
        {
            IsFront = true;
        }

        public void SetPlayerController(PlayerController playerController)
        {
            _playerController = playerController;
        }

        public void SetStageData(StageData stageData)
        {
            _stageData = stageData;
        }

        public void SetDoorStates(bool[] openedDoors)
        {
            _openedDoors = openedDoors ?? Array.Empty<bool>();
        }

        private bool IsClosedDoor(int x, int y, bool isFront, StageData stageData)
        {
            if (!isFront)
            {
                return false;
            }

            PositionData[] doors = stageData.Doors;
            if (doors == null)
            {
                return false;
            }

            for (int i = 0; i < doors.Length; i++)
            {
                PositionData door = doors[i];
                if (door == null || door.X != x || door.Y != y)
                {
                    continue;
                }

                return i >= _openedDoors.Length || !_openedDoors[i];
            }

            return false;
        }
    }
}
