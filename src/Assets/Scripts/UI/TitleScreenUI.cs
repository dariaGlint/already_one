using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlreadyOne.UI
{
    [DisallowMultipleComponent]
    public sealed class TitleScreenUI : MonoBehaviour
    {
        private static readonly Color s_backgroundColor = new Color32(0x1A, 0x1A, 0x1A, 0xFF);
        private static readonly Color s_textColor = Color.white;
        private static readonly Color s_buttonColor = new Color32(0x2A, 0x2A, 0x2A, 0xFF);

        private RectTransform _root;
        private Button _startButton;

        public event Action OnGameStarted;

        private void Awake()
        {
            UIBuilderUtility.EnsureEventSystem();
            UIBuilderUtility.EnsureCanvas(gameObject, 100);
            BuildUI();
        }

        private void OnDestroy()
        {
            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(HandleStartButtonClicked);
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

            Image background = UIBuilderUtility.CreateImage(transform, "Background", s_backgroundColor);
            _root = background.rectTransform;
            UIBuilderUtility.SetRectTransform(_root, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            Graphic titleText = UIBuilderUtility.CreateText(
                _root,
                "Title",
                "うらがえし迷宮",
                112,
                TextAnchor.MiddleCenter,
                s_textColor,
                FontStyle.Bold,
                false);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)titleText.transform,
                new Vector2(0.5f, 0.76f),
                new Vector2(0.5f, 0.76f),
                new Vector2(0.5f, 0.5f),
                new Vector2(960f, 140f),
                Vector2.zero);

            Graphic subtitleText = UIBuilderUtility.CreateText(
                _root,
                "Subtitle",
                "表では壁でも、うらがえすと道になる",
                42,
                TextAnchor.MiddleCenter,
                s_textColor,
                FontStyle.Normal,
                false);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)subtitleText.transform,
                new Vector2(0.5f, 0.61f),
                new Vector2(0.5f, 0.61f),
                new Vector2(0.5f, 0.5f),
                new Vector2(980f, 72f),
                Vector2.zero);

            _startButton = UIBuilderUtility.CreateButton(
                _root,
                "StartButton",
                "スタート",
                42,
                s_buttonColor,
                s_textColor);
            _startButton.onClick.AddListener(HandleStartButtonClicked);

            RectTransform buttonRect = (RectTransform)_startButton.transform;
            UIBuilderUtility.SetRectTransform(
                buttonRect,
                new Vector2(0.5f, 0.38f),
                new Vector2(0.5f, 0.38f),
                new Vector2(0.5f, 0.5f),
                new Vector2(360f, 96f),
                Vector2.zero);

            Graphic instructionText = UIBuilderUtility.CreateText(
                _root,
                "Instructions",
                "移動: WASD/矢印キー　反転: スペース",
                30,
                TextAnchor.MiddleCenter,
                s_textColor,
                FontStyle.Normal,
                false);
            UIBuilderUtility.SetRectTransform(
                (RectTransform)instructionText.transform,
                new Vector2(0.5f, 0.09f),
                new Vector2(0.5f, 0.09f),
                new Vector2(0.5f, 0.5f),
                new Vector2(900f, 54f),
                Vector2.zero);
        }

        private void HandleStartButtonClicked()
        {
            Hide();
            OnGameStarted?.Invoke();
        }
    }

    internal static class UIBuilderUtility
    {
        private static readonly Vector2 s_referenceResolution = new Vector2(1920f, 1080f);
        private static readonly Type s_tmpTextType = ResolveType("TMPro.TextMeshProUGUI, Unity.TextMeshPro", "TMPro.TextMeshProUGUI, TMPro");
        private static readonly Type s_tmpSettingsType = ResolveType("TMPro.TMP_Settings, Unity.TextMeshPro", "TMPro.TMP_Settings, TMPro");
        private static readonly PropertyInfo s_tmpDefaultFontAssetProperty = s_tmpSettingsType?.GetProperty("defaultFontAsset", BindingFlags.Public | BindingFlags.Static);
        private static readonly PropertyInfo s_tmpTextProperty = s_tmpTextType?.GetProperty("text", BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo s_tmpFontProperty = s_tmpTextType?.GetProperty("font", BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo s_tmpFontSizeProperty = s_tmpTextType?.GetProperty("fontSize", BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo s_tmpAlignmentProperty = s_tmpTextType?.GetProperty("alignment", BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo s_tmpWordWrapProperty = s_tmpTextType?.GetProperty("enableWordWrapping", BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo s_tmpOverflowModeProperty = s_tmpTextType?.GetProperty("overflowMode", BindingFlags.Public | BindingFlags.Instance);
        private static readonly PropertyInfo s_tmpFontStyleProperty = s_tmpTextType?.GetProperty("fontStyle", BindingFlags.Public | BindingFlags.Instance);

        private static Font s_defaultFont;
        private static bool? s_canUseTextMeshPro;

        public static void EnsureEventSystem()
        {
            if (EventSystem.current != null || UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        public static Canvas EnsureCanvas(GameObject host, int sortingOrder)
        {
            if (!host.TryGetComponent(out Canvas canvas))
            {
                canvas = host.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            if (!host.TryGetComponent(out CanvasScaler canvasScaler))
            {
                canvasScaler = host.AddComponent<CanvasScaler>();
            }

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = s_referenceResolution;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;

            if (!host.TryGetComponent(out GraphicRaycaster _))
            {
                host.AddComponent<GraphicRaycaster>();
            }

            return canvas;
        }

        public static RectTransform CreateRectTransform(Transform parent, string objectName)
        {
            GameObject childObject = new GameObject(objectName, typeof(RectTransform));
            childObject.transform.SetParent(parent, false);
            return childObject.GetComponent<RectTransform>();
        }

        public static Image CreateImage(Transform parent, string objectName, Color color)
        {
            RectTransform rectTransform = CreateRectTransform(parent, objectName);
            Image image = rectTransform.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Button CreateButton(
            Transform parent,
            string objectName,
            string label,
            int fontSize,
            Color backgroundColor,
            Color textColor)
        {
            Image image = CreateImage(parent, objectName, backgroundColor);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            ColorBlock colors = button.colors;
            colors.normalColor = backgroundColor;
            colors.highlightedColor = Color.Lerp(backgroundColor, Color.white, 0.14f);
            colors.pressedColor = Color.Lerp(backgroundColor, Color.black, 0.12f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = Color.Lerp(backgroundColor, Color.black, 0.35f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            Graphic labelGraphic = CreateText(
                button.transform,
                "Label",
                label,
                fontSize,
                TextAnchor.MiddleCenter,
                textColor,
                FontStyle.Bold,
                false);

            SetRectTransform(
                (RectTransform)labelGraphic.transform,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);

            return button;
        }

        public static Graphic CreateText(
            Transform parent,
            string objectName,
            string content,
            int fontSize,
            TextAnchor alignment,
            Color color,
            FontStyle fontStyle,
            bool enableWordWrapping)
        {
            RectTransform rectTransform = CreateRectTransform(parent, objectName);
            Graphic graphic = CanUseTextMeshPro()
                ? CreateTextMeshProGraphic(rectTransform.gameObject)
                : CreateLegacyTextGraphic(rectTransform.gameObject);

            SetText(graphic, content);
            SetFontSize(graphic, fontSize);
            SetAlignment(graphic, alignment);
            SetColor(graphic, color);
            SetFontStyle(graphic, fontStyle);
            SetWordWrapping(graphic, enableWordWrapping);
            graphic.raycastTarget = false;
            return graphic;
        }

        public static void SetRectTransform(
            RectTransform rectTransform,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 sizeDelta,
            Vector2 anchoredPosition)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public static void SetText(Graphic graphic, string content)
        {
            if (graphic is Text legacyText)
            {
                legacyText.text = content;
                return;
            }

            if (IsTextMeshProGraphic(graphic))
            {
                s_tmpTextProperty?.SetValue(graphic, content, null);
            }
        }

        public static void SetFontSize(Graphic graphic, int fontSize)
        {
            if (graphic is Text legacyText)
            {
                legacyText.fontSize = fontSize;
                return;
            }

            if (IsTextMeshProGraphic(graphic))
            {
                s_tmpFontSizeProperty?.SetValue(graphic, (float)fontSize, null);
            }
        }

        public static void SetAlignment(Graphic graphic, TextAnchor alignment)
        {
            if (graphic is Text legacyText)
            {
                legacyText.alignment = alignment;
                return;
            }

            if (IsTextMeshProGraphic(graphic) && s_tmpAlignmentProperty != null)
            {
                string alignmentName = alignment switch
                {
                    TextAnchor.UpperLeft => "TopLeft",
                    TextAnchor.UpperCenter => "Top",
                    TextAnchor.UpperRight => "TopRight",
                    TextAnchor.MiddleLeft => "Left",
                    TextAnchor.MiddleCenter => "Center",
                    TextAnchor.MiddleRight => "Right",
                    TextAnchor.LowerLeft => "BottomLeft",
                    TextAnchor.LowerCenter => "Bottom",
                    _ => "BottomRight"
                };

                object alignmentValue = Enum.Parse(s_tmpAlignmentProperty.PropertyType, alignmentName);
                s_tmpAlignmentProperty.SetValue(graphic, alignmentValue, null);
            }
        }

        public static void SetColor(Graphic graphic, Color color)
        {
            if (graphic != null)
            {
                graphic.color = color;
            }
        }

        public static void SetFontStyle(Graphic graphic, FontStyle fontStyle)
        {
            if (graphic is Text legacyText)
            {
                legacyText.fontStyle = fontStyle;
                return;
            }

            if (IsTextMeshProGraphic(graphic) && s_tmpFontStyleProperty != null)
            {
                string styleName = fontStyle switch
                {
                    FontStyle.Bold => "Bold",
                    FontStyle.Italic => "Italic",
                    FontStyle.BoldAndItalic => "Bold",
                    _ => "Normal"
                };

                object styleValue = Enum.Parse(s_tmpFontStyleProperty.PropertyType, styleName);
                s_tmpFontStyleProperty.SetValue(graphic, styleValue, null);
            }
        }

        private static bool CanUseTextMeshPro()
        {
            if (s_canUseTextMeshPro.HasValue)
            {
                return s_canUseTextMeshPro.Value;
            }

            s_canUseTextMeshPro = s_tmpTextType != null
                && s_tmpDefaultFontAssetProperty != null
                && s_tmpDefaultFontAssetProperty.GetValue(null, null) != null;

            return s_canUseTextMeshPro.Value;
        }

        private static Graphic CreateTextMeshProGraphic(GameObject host)
        {
            Component textComponent = host.AddComponent(s_tmpTextType);
            if (s_tmpFontProperty != null && s_tmpDefaultFontAssetProperty != null)
            {
                object defaultFontAsset = s_tmpDefaultFontAssetProperty.GetValue(null, null);
                s_tmpFontProperty.SetValue(textComponent, defaultFontAsset, null);
            }

            SetTextOverflow(textComponent);
            return textComponent as Graphic;
        }

        private static Graphic CreateLegacyTextGraphic(GameObject host)
        {
            Text text = host.AddComponent<Text>();
            text.font = GetDefaultFont();
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static void SetTextOverflow(Component textComponent)
        {
            if (s_tmpOverflowModeProperty == null || textComponent == null)
            {
                return;
            }

            object overflowValue = Enum.Parse(s_tmpOverflowModeProperty.PropertyType, "Overflow");
            s_tmpOverflowModeProperty.SetValue(textComponent, overflowValue, null);
        }

        private static void SetWordWrapping(Graphic graphic, bool enableWordWrapping)
        {
            if (graphic is Text legacyText)
            {
                legacyText.horizontalOverflow = enableWordWrapping ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;
                legacyText.verticalOverflow = VerticalWrapMode.Overflow;
                return;
            }

            if (IsTextMeshProGraphic(graphic))
            {
                s_tmpWordWrapProperty?.SetValue(graphic, enableWordWrapping, null);
            }
        }

        private static Font GetDefaultFont()
        {
            if (s_defaultFont != null)
            {
                return s_defaultFont;
            }

            s_defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (s_defaultFont == null)
            {
                s_defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return s_defaultFont;
        }

        private static bool IsTextMeshProGraphic(Graphic graphic)
        {
            return graphic != null && s_tmpTextType != null && s_tmpTextType.IsInstanceOfType(graphic);
        }

        private static Type ResolveType(params string[] typeNames)
        {
            for (int i = 0; i < typeNames.Length; i++)
            {
                Type resolvedType = Type.GetType(typeNames[i]);
                if (resolvedType != null)
                {
                    return resolvedType;
                }
            }

            return null;
        }
    }
}
