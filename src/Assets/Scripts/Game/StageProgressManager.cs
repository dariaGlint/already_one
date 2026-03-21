using System;
using UnityEngine;

namespace AlreadyOne.Game
{
    [DisallowMultipleComponent]
    public sealed class StageProgressManager : MonoBehaviour
    {
        [Header("Progress")]
        [SerializeField] [Min(1)] private int _totalStages = 10;

        public int CurrentStageNumber { get; private set; } = 1;

        public bool IsAllCleared { get; private set; }

        public event Action<int> OnStageAdvanced;
        public event Action OnAllStagesCleared;

        public void AdvanceToNextStage()
        {
            if (IsAllCleared)
            {
                return;
            }

            int totalStages = Mathf.Max(1, _totalStages);
            if (CurrentStageNumber >= totalStages)
            {
                IsAllCleared = true;
                OnAllStagesCleared?.Invoke();
                return;
            }

            CurrentStageNumber++;
            OnStageAdvanced?.Invoke(CurrentStageNumber);
        }

        public void ResetProgress()
        {
            CurrentStageNumber = 1;
            IsAllCleared = false;
        }

        private void OnValidate()
        {
            _totalStages = Mathf.Max(1, _totalStages);
        }
    }
}
