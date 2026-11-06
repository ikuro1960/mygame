using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailAppController : MonoBehaviour
{
    [System.Serializable]
    public class MailEntry
    {
        public string sender;
        public string subject;
        public string date;
        public string email;
        [TextArea(3, 12)]
        public string body;
    }

    [Header("メールデータ（メール一覧の上から順に対応）")]
    [SerializeField] private List<MailEntry> mails = new List<MailEntry>();

    private GameObject detailPanel;
    private TextMeshProUGUI subjectText;
    private TextMeshProUGUI senderText;
    private TextMeshProUGUI dateText;
    private TextMeshProUGUI bodyText;
    private ScrollRect bodyScroll;

    void Start()
    {
        Transform listPanel = transform.Find("MailList_Panel");
        detailPanel = transform.Find("MailDetail_Panel")?.gameObject;

        if (listPanel == null || detailPanel == null) return;

        TMP_FontAsset font = GetListFont(listPanel);

        var panelImage = detailPanel.GetComponent<Image>();
        if (panelImage != null) panelImage.color = new Color(1f, 1f, 1f, 0.97f);

        BuildDetailView(detailPanel.transform, font);
        FixAllTexts();
        BindMailButtons(listPanel);
    }

    private const char LongSoundMark = 'ー';
    private const char HorizontalBar = '―';

    private static string FixLongMarks(string s)
    {
        return string.IsNullOrEmpty(s) ? s : s.Replace(LongSoundMark, HorizontalBar);
    }

    private void FixAllTexts()
    {
        foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (!string.IsNullOrEmpty(tmp.text) && tmp.text.IndexOf(LongSoundMark) >= 0)
                tmp.text = FixLongMarks(tmp.text);
        }
    }

    private TMP_FontAsset GetListFont(Transform listPanel)
    {
        foreach (var tmp in listPanel.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp.font != null) return tmp.font;
        }
        return TMP_Settings.defaultFontAsset;
    }

    private void BuildDetailView(Transform parent, TMP_FontAsset font)
    {
        var contentGo = new GameObject("MailDetail_Content", typeof(RectTransform));
        contentGo.transform.SetParent(parent, false);
        var contentRt = contentGo.GetComponent<RectTransform>();
        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = Vector2.one;
        contentRt.offsetMin = new Vector2(30, 30);
        contentRt.offsetMax = new Vector2(-30, -90);

        subjectText = CreateText("SubjectText", contentRt, font, 30, TextAlignmentOptions.TopLeft, new Color(0.1f, 0.1f, 0.1f));
        subjectText.fontStyle = FontStyles.Bold;
        var subjectRt = subjectText.rectTransform;
        subjectRt.anchorMin = new Vector2(0, 1);
        subjectRt.anchorMax = new Vector2(1, 1);
        subjectRt.pivot = new Vector2(0.5f, 1);
        subjectRt.anchoredPosition = new Vector2(0, -8);
        subjectRt.sizeDelta = new Vector2(0, 80);

        var headerBoxGo = new GameObject("HeaderBox", typeof(RectTransform), typeof(Image));
        headerBoxGo.transform.SetParent(contentRt, false);
        var headerBoxRt = headerBoxGo.GetComponent<RectTransform>();
        headerBoxRt.anchorMin = new Vector2(0, 1);
        headerBoxRt.anchorMax = new Vector2(1, 1);
        headerBoxRt.pivot = new Vector2(0.5f, 1);
        headerBoxRt.anchoredPosition = new Vector2(0, -96);
        headerBoxRt.sizeDelta = new Vector2(-20, 92);
        var headerBoxImg = headerBoxGo.GetComponent<Image>();
        headerBoxImg.color = new Color(0.93f, 0.93f, 0.93f, 1f);
        headerBoxImg.raycastTarget = false;

        senderText = CreateText("SenderText", headerBoxRt, font, 20, TextAlignmentOptions.TopLeft, new Color(0.2f, 0.2f, 0.2f));
        var senderRt = senderText.rectTransform;
        senderRt.anchorMin = new Vector2(0, 1);
        senderRt.anchorMax = new Vector2(1, 1);
        senderRt.pivot = new Vector2(0.5f, 1);
        senderRt.anchoredPosition = new Vector2(0, -10);
        senderRt.sizeDelta = new Vector2(-24, 42);

        dateText = CreateText("DateText", headerBoxRt, font, 18, TextAlignmentOptions.TopLeft, new Color(0.45f, 0.45f, 0.45f));
        var dateRt = dateText.rectTransform;
        dateRt.anchorMin = new Vector2(0, 1);
        dateRt.anchorMax = new Vector2(1, 1);
        dateRt.pivot = new Vector2(0.5f, 1);
        dateRt.anchoredPosition = new Vector2(0, -58);
        dateRt.sizeDelta = new Vector2(-24, 26);

        var dividerGo = new GameObject("Divider", typeof(RectTransform), typeof(Image));
        dividerGo.transform.SetParent(contentRt, false);
        var dividerRt = dividerGo.GetComponent<RectTransform>();
        dividerRt.anchorMin = new Vector2(0, 1);
        dividerRt.anchorMax = new Vector2(1, 1);
        dividerRt.pivot = new Vector2(0.5f, 1);
        dividerRt.anchoredPosition = new Vector2(0, -200);
        dividerRt.sizeDelta = new Vector2(0, 4);
        var dividerImg = dividerGo.GetComponent<Image>();
        dividerImg.color = new Color(0.55f, 0.55f, 0.55f, 1f);
        dividerImg.raycastTarget = false;

        var scrollGo = new GameObject("BodyScroll", typeof(RectTransform), typeof(ScrollRect));
        scrollGo.transform.SetParent(contentRt, false);
        var scrollRt = scrollGo.GetComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = new Vector2(0, 15);
        scrollRt.offsetMax = new Vector2(0, -216);

        bodyScroll = scrollGo.GetComponent<ScrollRect>();
        bodyScroll.horizontal = false;
        bodyScroll.vertical = true;
        bodyScroll.movementType = ScrollRect.MovementType.Clamped;
        bodyScroll.scrollSensitivity = 25f;

        var viewportGo = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        viewportGo.transform.SetParent(scrollRt, false);
        var viewportRt = viewportGo.GetComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewportGo.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        bodyText = CreateText("BodyText", viewportRt, font, 22, TextAlignmentOptions.TopLeft, new Color(0.15f, 0.15f, 0.15f));
        bodyText.margin = new Vector4(10, 10, 10, 10);
        var bodyRt = bodyText.rectTransform;
        bodyRt.anchorMin = new Vector2(0, 1);
        bodyRt.anchorMax = new Vector2(1, 1);
        bodyRt.pivot = new Vector2(0.5f, 1);
        bodyRt.anchoredPosition = Vector2.zero;
        bodyRt.sizeDelta = new Vector2(0, 0);
        bodyRt.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        bodyScroll.viewport = viewportRt;
        bodyScroll.content = bodyRt;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, TMP_FontAsset font, float fontSize, TextAlignmentOptions alignment, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<TextMeshProUGUI>();
        text.font = font != null ? font : TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private void BindMailButtons(Transform listPanel)
    {
        int index = 0;
        foreach (var button in listPanel.GetComponentsInChildren<Button>(true))
        {
            if (!button.name.StartsWith("MailItem_Button")) continue;
            int mailIndex = index++;
            button.onClick.AddListener(() => OpenMail(mailIndex));
        }
    }

    public void OpenMail(int index)
    {
        if (index < 0 || index >= mails.Count) return;

        var mail = mails[index];
        subjectText.text = FixLongMarks(string.IsNullOrEmpty(mail.subject) ? "(件名なし)" : mail.subject);
        senderText.text = FixLongMarks("差出人: " + mail.sender
            + (string.IsNullOrEmpty(mail.email) ? "" : $" <{mail.email}>"));
        dateText.text = FixLongMarks(string.IsNullOrEmpty(mail.date) ? "" : $"受信日時: {mail.date}");
        bodyText.text = FixLongMarks(mail.body);

        bodyScroll.verticalNormalizedPosition = 1f;
        if (detailPanel != null) detailPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }
}
