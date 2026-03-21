using UnityEngine;

namespace AlreadyOne.Data
{
    public static class StageLoader
    {
        public static StageData LoadStage(int stageNumber)
        {
            string resourcePath = $"Stages/stage_{stageNumber:00}";
            TextAsset stageAsset = Resources.Load<TextAsset>(resourcePath);
            if (stageAsset == null)
            {
                Debug.LogError($"Failed to load stage data from Resources/{resourcePath}.");
                return null;
            }

            return LoadStageFromJson(stageAsset.text);
        }

        public static StageData LoadStageFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("Stage JSON is null or empty.");
                return null;
            }

            return JsonUtility.FromJson<StageData>(json);
        }
    }
}
