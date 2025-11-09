using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CreditsScroller : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The visible area. Add a RectMask2D here to hide overflow.")]
    [SerializeField] private RectTransform viewport;
    [Tooltip("The content that will be moved upwards. Will be auto-created if missing.")]
    [SerializeField] private RectTransform content;
    [Tooltip("Optional: font to use for generated Text components.")]
    [SerializeField] private Font defaultFont;

    [Header("Appearance")]
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.75f);
    [SerializeField] private Color headerColor = Color.white;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private int headerFontSize = 36;
    [SerializeField] private int textFontSize = 28;
    [SerializeField] private float lineSpacing = 12f;
    [SerializeField] private float sidePadding = 40f;

    [Header("Content Width")]
    [SerializeField] private ContentWidthMode contentWidthMode = ContentWidthMode.MatchViewport;
    [SerializeField] [Min(0f)] private float fixedContentWidth = 900f;
    [Tooltip("Adds a LayoutElement with min/preferred width so text does not collapse.")]
    [SerializeField] private bool enforceMinWidth = true;

    [Header("Scroll")]
    [SerializeField] private float startDelay = 1.0f;
    [SerializeField] private float endDelay = 1.0f;
    [SerializeField] private float pixelsPerSecond = 80f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool anyKeySkipsToEnd = true;

    [Header("Credits Data")]
    [SerializeField] private List<CreditSection> sections = new List<CreditSection>
    {
        new CreditSection
        {
            header = "Development",
            people = new List<CreditPerson>
            {
                new CreditPerson { leftText = "Programming", rightText = "Alice Example"},
                new CreditPerson { leftText = "Design", rightText = "Bob Example"},
            }
        },
        new CreditSection
        {
            header = "Art",
            people = new List<CreditPerson>
            {
                new CreditPerson { leftText = "2D Art", rightText = "Carol Example"},
                new CreditPerson { leftText = "3D Art", rightText = "Dave Example"},
            }
        },
    };

    public event Action OnFinished;

    private float _timer;
    private float _currentY;
    private float _contentHeight;
    private bool _isRunning;
    private bool _built;

    [Serializable]
    public class CreditPerson
    {
        [Tooltip("Left column text (e.g., role)")]
        public string leftText;
        [Tooltip("Right column text (e.g., name)")]
        public string rightText;
        [Tooltip("Optional image (e.g., logo or portrait)")]
        public Sprite image;
        [Tooltip("Row height (pixels).")]
        public float rowHeight = 64f;
    }

    [Serializable]
    public class CreditSection
    {
        public string header;
        public List<CreditPerson> people = new List<CreditPerson>();
    }

    public enum ContentWidthMode
    {
        MatchViewport,
        Fixed
    }

    private void Reset()
    {
        viewport = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        EnsureHierarchy();
    }

    private void Start()
    {
        BuildIfNeeded();

        if (autoStart)
        {
            Begin();
        }
    }

    private void OnValidate()
    {
        // Apply width changes live in editor
        if (viewport == null) viewport = GetComponent<RectTransform>();
        if (content != null)
        {
            ApplyContentWidth();
        }
    }

    private void Update()
    {
        if (!_isRunning) return;

        // Initial delay before scrolling starts
        if (_timer < startDelay)
        {
            _timer += Time.deltaTime;
            return;
        }

        if (anyKeySkipsToEnd && Input.anyKeyDown)
        {
            // Jump to end
            _currentY = viewport.rect.height + endDelay * pixelsPerSecond;
        }

        _currentY += pixelsPerSecond * Time.deltaTime;
        content.anchoredPosition = new Vector2(0f, _currentY);

        // Finish when top of content has passed the top of the viewport + end delay padding
        float finishAt = viewport.rect.height + (endDelay * pixelsPerSecond);
        if (_currentY >= finishAt)
        {
            if (loop)
            {
                Restart();
            }
            else
            {
                _isRunning = false;
                OnFinished?.Invoke();
            }
        }
    }

    public void Begin()
    {
        BuildIfNeeded();
        PrepareForScroll();
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void Restart()
    {
        PrepareForScroll();
        _isRunning = true;
    }

    public void SetSections(List<CreditSection> newSections, bool rebuild = true)
    {
        sections = newSections ?? new List<CreditSection>();
        if (rebuild)
        {
            Rebuild();
        }
    }

    public void AddPerson(string sectionHeader, string left, string right, Sprite image = null, float rowHeight = 64f)
    {
        var section = sections.Find(s => string.Equals(s.header, sectionHeader, StringComparison.OrdinalIgnoreCase));
        if (section == null)
        {
            section = new CreditSection { header = sectionHeader };
            sections.Add(section);
        }
        section.people.Add(new CreditPerson { leftText = left, rightText = right, image = image, rowHeight = rowHeight });
    }

    public void Rebuild()
    {
        ClearContent();
        _built = false;
        BuildIfNeeded();
        PrepareForScroll();
    }

    private void EnsureHierarchy()
    {
        if (viewport == null) viewport = GetComponent<RectTransform>();

        // Add a semi-transparent background to the viewport (optional)
        var image = viewport.GetComponent<Image>();
        if (image == null) image = viewport.gameObject.AddComponent<Image>();
        image.color = backgroundColor;

        // Ensure a mask to clip overflow
        if (viewport.GetComponent<RectMask2D>() == null)
        {
            viewport.gameObject.AddComponent<RectMask2D>();
        }

        // Ensure content rect
        if (content == null)
        {
            var go = new GameObject("Content", typeof(RectTransform));
            go.transform.SetParent(viewport, false);
            content = go.GetComponent<RectTransform>();
        }

        // Configure content anchors/pivot for bottom-based scrolling (may be overridden by width mode)
        content.pivot = new Vector2(0.5f, 0f);
        ApplyContentWidth();

        // Add layout components to content
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = lineSpacing;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childForceExpandHeight = false;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childControlWidth = true;
        vlg.padding = new RectOffset(Mathf.RoundToInt(sidePadding), Mathf.RoundToInt(sidePadding), Mathf.RoundToInt(sidePadding), Mathf.RoundToInt(sidePadding));

        var fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        if (enforceMinWidth)
        {
            var le = content.GetComponent<LayoutElement>();
            if (le == null) le = content.gameObject.AddComponent<LayoutElement>();
            le.minWidth = GetEffectiveWidth();
            le.preferredWidth = GetEffectiveWidth();
        }
    }

    private void ApplyContentWidth()
    {
        if (contentWidthMode == ContentWidthMode.MatchViewport)
        {
            // Stretch to viewport width
            content.anchorMin = new Vector2(0f, 0f);
            content.anchorMax = new Vector2(1f, 0f);
            content.sizeDelta = new Vector2(0f, 0f);
        }
        else
        {
            // Fixed width centered
            content.anchorMin = new Vector2(0.5f, 0f);
            content.anchorMax = new Vector2(0.5f, 0f);
            content.sizeDelta = new Vector2(fixedContentWidth, 0f);
        }
        content.anchoredPosition = new Vector2(0f, 0f);

        // Update enforced width if needed
        if (enforceMinWidth)
        {
            var le = content.GetComponent<LayoutElement>();
            if (le != null)
            {
                le.minWidth = GetEffectiveWidth();
                le.preferredWidth = GetEffectiveWidth();
            }
        }
    }

    private float GetEffectiveWidth()
    {
        if (contentWidthMode == ContentWidthMode.MatchViewport && viewport != null)
        {
            return viewport.rect.width - sidePadding * 2f;
        }
        return fixedContentWidth - sidePadding * 2f;
    }

    private void BuildIfNeeded()
    {
        if (_built) return;

        // Generate UI from data
        foreach (var section in sections)
        {
            if (!string.IsNullOrWhiteSpace(section.header))
            {
                CreateHeader(section.header);
            }

            foreach (var person in section.people)
            {
                CreatePersonRow(person);
            }

            // Add a small spacer between sections
            CreateSpacer(Mathf.Max(8f, lineSpacing));
        }

        // Recalculate sizes
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        _contentHeight = content.rect.height;
        _built = true;
    }

    private void PrepareForScroll()
    {
        _timer = 0f;

        // Start with content entirely below the viewport
        _currentY = -_contentHeight; // since content pivot is bottom, y = -height puts bottom of content below bottom of viewport
        content.anchoredPosition = new Vector2(0f, _currentY);
    }

    private void ClearContent()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            var child = content.GetChild(i);
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
    }

    private void CreateHeader(string text)
    {
        var go = CreateRow("Header", preferredHeight: Mathf.Max(52f, headerFontSize + lineSpacing));
        var txt = CreateText(go.transform, text, headerFontSize, headerColor, FontStyle.Bold, TextAnchor.MiddleCenter);
        txt.alignment = TextAnchor.MiddleCenter;
    }

    private void CreatePersonRow(CreditPerson person)
    {
        float h = Mathf.Max(40f, person.rowHeight);
        var row = CreateRow("Row", preferredHeight: h, horizontal: true, alignMiddle: true);

        if (person.image != null)
        {
            var imgGO = new GameObject("Image", typeof(RectTransform), typeof(LayoutElement), typeof(Image));
            imgGO.transform.SetParent(row.transform, false);
            var imgRect = imgGO.GetComponent<RectTransform>();
            imgRect.sizeDelta = new Vector2(h, h);
            var le = imgGO.GetComponent<LayoutElement>();
            le.preferredWidth = h;
            le.preferredHeight = h;
            le.minHeight = h;
            var img = imgGO.GetComponent<Image>();
            img.sprite = person.image;
            img.preserveAspect = true;
        }

        // Left (role)
        var left = CreateText(row.transform, person.leftText, textFontSize, textColor, FontStyle.Normal, TextAnchor.MiddleLeft);
        var leftLE = left.gameObject.AddComponent<LayoutElement>();
        leftLE.flexibleWidth = 1f;
        leftLE.minHeight = h;

        // Right (name)
        var right = CreateText(row.transform, person.rightText, textFontSize, textColor, FontStyle.Normal, TextAnchor.MiddleRight);
        var rightLE = right.gameObject.AddComponent<LayoutElement>();
        rightLE.flexibleWidth = 1f;
        rightLE.minHeight = h;
    }

    private void CreateSpacer(float height)
    {
        CreateRow("Spacer", preferredHeight: height);
    }

    private GameObject CreateRow(string name, float preferredHeight, bool horizontal = false, bool alignMiddle = false)
    {
        var row = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
        row.transform.SetParent(content, false);

        var rect = row.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(0, preferredHeight);

        var le = row.GetComponent<LayoutElement>();
        le.preferredHeight = preferredHeight;
        le.minHeight = preferredHeight;

        if (horizontal)
        {
            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 12f;
            hlg.childAlignment = alignMiddle ? TextAnchor.MiddleCenter : TextAnchor.UpperLeft;
            hlg.childForceExpandWidth = true;
            hlg.childControlWidth = true;
            hlg.childForceExpandHeight = false;
            hlg.childControlHeight = true;
            hlg.padding = new RectOffset(0, 0, 0, 0);
        }

        return row;
    }

    private Text CreateText(Transform parent, string value, int fontSize, Color color, FontStyle style, TextAnchor anchor)
    {
        var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        var text = go.GetComponent<Text>();
        text.text = value ?? string.Empty;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;

        if (defaultFont != null) text.font = defaultFont;

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return text;
    }
}
