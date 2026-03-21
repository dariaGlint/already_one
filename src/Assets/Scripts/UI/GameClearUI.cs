using System;
using UnityEngine;
using UnityEngine.UI;

namespace AlreadyOne.UI
{
    [DisallowMultipleComponent]
    public sealed class GameClearUI : MonoBehaviour
    {
        private static readonly Color s_overlayColor = new Color32(0x1A, 0x1A, 0x1A, 0xF2);
        private static readonly Color s_panelColor = new Color32(0x24, 0x24, 0x24, 0xFF);
        private static readonly Color s_textColor = Color.white;
        private static readonly Color s_buttonColor = new Color32(0x2A, 0x2A, 0x2A, 0xFF);

        private RectTransform _root;
        private Button _returnButton;

        public event Action OnReturnToTitleRequested;

        private void Awake()
        {
            UIBuilderUtility.EnsureEventSystem();
            UIBuilderUtility.EnsureCanvas(gameObject, 120);
            BuildUI();
            Hide();
        }

        private void OnDestroy()
        {
            if (_returnButton != null)
            {
                _returnButton.onClick.RemoveListener(HandleReturnButtonClicked);
            }
        }

        public void Show()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(true);
            }
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
                new Vector2(860f, 420f),
                Vector2.zero);

            Graphic clearText = UIBuilderUtility.CreateText(
                panelRect,
                "GameClearText",
                "全ステージクリア！おめでとう！",
                68,
                TextAnchor.MiddleCenter,
                s_textColor,
                FontStyle.Bold,
                true);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)clearText.transform,
                new Vector2(0.5f, 0.64f),
                new Vector2(0.5f, 0.64f),
                new Vector2(0.5f, 0.5f),
                new Vector2(720f, 150f),
                Vector2.zero);

            _returnButton = UIBuilderUtility.CreateButton(
                panelRect,
                "ReturnToTitleButton",
                "タイトルに戻る",
                34,
                s_buttonColor,
                s_textColor);
            _returnButton.onClick.AddListener(HandleReturnButtonClicked);

            UIBuilderUtility.SetRectTransform(
                (RectTransform)_returnButton.transform,
                new Vector2(0.5f, 0.26f),
                new Vector2(0.5f, 0.26f),
                new Vector2(0.5f, 0.5f),
                new Vector2(360f, 86f),
                Vector2.zero);
        }

        private void HandleReturnButtonClicked()
        {
            Hide();
            OnReturnToTitleRequested?.Invoke();
        }
    }
}
