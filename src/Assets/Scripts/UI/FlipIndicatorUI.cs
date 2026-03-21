using UnityEngine;
using UnityEngine.UI;

namespace AlreadyOne.UI
{
    [DisallowMultipleComponent]
    public sealed class FlipIndicatorUI : MonoBehaviour
    {
        private static readonly Color s_frontBackgroundColor = new Color32(0x00, 0x00, 0x00, 0x99);
        private static readonly Color s_frontTextColor = Color.white;
        private static readonly Color s_backBackgroundColor = Color.white;
        private static readonly Color s_backTextColor = new Color32(0x1A, 0x1A, 0x1A, 0xFF);

        private RectTransform _root;
        private Image _backgroundImage;
        private Graphic _faceText;

        private void Awake()
        {
            UIBuilderUtility.EnsureCanvas(gameObject, 20);
            BuildUI();
            Hide();
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

        public void SetFace(bool isFront)
        {
            if (_backgroundImage == null || _faceText == null)
            {
                return;
            }

            UIBuilderUtility.SetText(_faceText, isFront ? "表" : "裏");
            _backgroundImage.color = isFront ? s_frontBackgroundColor : s_backBackgroundColor;
            UIBuilderUtility.SetColor(_faceText, isFront ? s_frontTextColor : s_backTextColor);
        }

        private void BuildUI()
        {
            if (_root != null)
            {
                return;
            }

            _backgroundImage = UIBuilderUtility.CreateImage(transform, "Indicator", s_frontBackgroundColor);
            _root = _backgroundImage.rectTransform;
            UIBuilderUtility.SetRectTransform(
                _root,
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(160f, 96f),
                new Vector2(-42f, -42f));

            _faceText = UIBuilderUtility.CreateText(
                _root,
                "FaceText",
                "表",
                54,
                TextAnchor.MiddleCenter,
                s_frontTextColor,
                FontStyle.Bold,
                false);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)_faceText.transform,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
        }
    }
}
