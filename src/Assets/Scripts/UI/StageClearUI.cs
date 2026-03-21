using System;
using UnityEngine;
using UnityEngine.UI;

namespace AlreadyOne.UI
{
    [DisallowMultipleComponent]
    public sealed class StageClearUI : MonoBehaviour
    {
        private static readonly Color s_overlayColor = new Color32(0x1A, 0x1A, 0x1A, 0xE6);
        private static readonly Color s_panelColor = new Color32(0x23, 0x23, 0x23, 0xF0);
        private static readonly Color s_textColor = Color.white;
        private static readonly Color s_buttonColor = new Color32(0x2A, 0x2A, 0x2A, 0xFF);

        private RectTransform _root;
        private Graphic _stageNumberText;
        private Button _nextStageButton;

        public event Action OnNextStageRequested;

        private void Awake()
        {
            UIBuilderUtility.EnsureEventSystem();
            UIBuilderUtility.EnsureCanvas(gameObject, 110);
            BuildUI();
            Hide();
        }

        private void OnDestroy()
        {
            if (_nextStageButton != null)
            {
                _nextStageButton.onClick.RemoveListener(HandleNextStageButtonClicked);
            }
        }

        public void Show(int clearedStageNumber)
        {
            if (_root == null)
            {
                return;
            }

            UIBuilderUtility.SetText(_stageNumberText, $"ステージ {clearedStageNumber}");
            _root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        private void BuildUI()
        {
            if (_root != null)
            {
                return;
            }

            Image overlay = UIBuilderUtility.CreateImage(transform, "Overlay", s_overlayColor);
            _root = overlay.rectTransform;
            UIBuilderUtility.SetRectTransform(_root, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            Image panel = UIBuilderUtility.CreateImage(_root, "Panel", s_panelColor);
            RectTransform panelRect = panel.rectTransform;
            UIBuilderUtility.SetRectTransform(
                panelRect,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(760f, 420f),
                Vector2.zero);

            Graphic clearText = UIBuilderUtility.CreateText(
                panelRect,
                "ClearText",
                "ステージクリア！",
                78,
                TextAnchor.MiddleCenter,
                s_textColor,
                FontStyle.Bold,
                false);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)clearText.transform,
                new Vector2(0.5f, 0.76f),
                new Vector2(0.5f, 0.76f),
                new Vector2(0.5f, 0.5f),
                new Vector2(620f, 110f),
                Vector2.zero);

            _stageNumberText = UIBuilderUtility.CreateText(
                panelRect,
                "StageNumberText",
                "ステージ 1",
                42,
                TextAnchor.MiddleCenter,
                s_textColor,
                FontStyle.Normal,
                false);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)_stageNumberText.transform,
                new Vector2(0.5f, 0.54f),
                new Vector2(0.5f, 0.54f),
                new Vector2(0.5f, 0.5f),
                new Vector2(480f, 70f),
                Vector2.zero);

            _nextStageButton = UIBuilderUtility.CreateButton(
                panelRect,
                "NextStageButton",
                "次のステージへ",
                34,
                s_buttonColor,
                s_textColor);
            _nextStageButton.onClick.AddListener(HandleNextStageButtonClicked);

            UIBuilderUtility.SetRectTransform(
                (RectTransform)_nextStageButton.transform,
                new Vector2(0.5f, 0.24f),
                new Vector2(0.5f, 0.24f),
                new Vector2(0.5f, 0.5f),
                new Vector2(360f, 86f),
                Vector2.zero);
        }

        private void HandleNextStageButtonClicked()
        {
            Hide();
            OnNextStageRequested?.Invoke();
        }
    }
}
