using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private AudioClip backButtonHoverSound;
    private const float rowHeight = 44f;
    private const float fontSize = 36f;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }
        StartCoroutine(ApiRequest.LeaderboardCoroutine(OnResponse));
    }

    private void OnResponse(LeaderboardResponse response)
    {
        if (response == null || !response.status) return;
        BuildUI(response.data);
    }

    private void BuildUI(List<LeaderboardEntry> entries)
    {
        var canvasGo = new GameObject("LeaderboardCanvas");
        canvasGo.transform.SetParent(null);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Фон на весь экран
        var bg = MakeRect(canvasGo.transform, "BG");
        Stretch(bg);
        bg.gameObject.AddComponent<Image>().color = new Color(0.07058824f, 0.09411765f, 0.18823529f, 1f);

        // Кнопка Back
        AddBackButton(bg);

        // Центральная колонка
        var column = MakeRect(bg, "Column");
        column.anchorMin = new Vector2(0.3f, 0.05f);
        column.anchorMax = new Vector2(0.7f, 0.95f);
        column.offsetMin = column.offsetMax = Vector2.zero;

        var vlg = column.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6f;
        vlg.padding = new RectOffset(0, 0, 0, 0);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        // Заголовок
        AddTitle(column);

        // Шапка таблицы
        AddRow(column, "#", "ИГРОК", "ПОБЕДЫ", "ПОРАЖЕНИЯ", new Color(0.07058824f, 0.09411765f, 0.18823529f), true, new Color(0.953f, 0.910f, 0.827f));

        // Строки
        for (int i = 0; i < entries.Count; i++)
        {
            var rowBg = new Color(0.953f, 0.910f, 0.827f);
            AddRow(column, (i + 1).ToString(), entries[i].username, entries[i].wins.ToString(), entries[i].losses.ToString(), new Color(0.753f, 0.157f, 0.141f), false, rowBg);
        }
    }

    private void AddTitle(RectTransform parent)
    {
        var go = new GameObject("Title");
        go.transform.SetParent(parent, false);
        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = fontSize * 1.5f + 10f;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "LEADERBOARD";
        tmp.fontSize = fontSize * 1.5f;
        tmp.color = new Color(0.753f, 0.157f, 0.141f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        if (font != null) tmp.font = font;
    }

    private void AddRow(RectTransform parent, string rank, string username, string wins, string losses,
        Color textColor, bool isHeader, Color bgColor = default)
    {
        var row = new GameObject("Row");
        row.transform.SetParent(parent, false);
        var rowImg = row.AddComponent<Image>();
        rowImg.color = bgColor;
        var le = row.AddComponent<LayoutElement>();
        le.preferredHeight = rowHeight;
        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(16, 16, 4, 4);
        hlg.spacing = 8f;
        hlg.childForceExpandHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;

        MakeCell(row.transform, rank, textColor, isHeader, 60f);
        MakeCell(row.transform, username, textColor, isHeader, -1f);
        MakeCell(row.transform, wins, textColor, isHeader, -1f);
        MakeCell(row.transform, losses, textColor, isHeader, -1f);
    }

    private void MakeCell(Transform parent, string text, Color color, bool bold, float width)
    {
        var go = new GameObject("Cell");
        go.transform.SetParent(parent, false);
        var le = go.AddComponent<LayoutElement>();
        if (width < 0) le.flexibleWidth = 1f;
        else le.preferredWidth = width;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        if (font != null) tmp.font = font;
    }

    private void AddBackButton(RectTransform parent)
    {
        var go = new GameObject("BackButton");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(160f, 50f);

        var btn = go.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.onClick.AddListener(() => SceneManager.LoadScene("LoginScene"));

        var hoverSound = go.AddComponent<ButtonHoverSound>();
        hoverSound.SetHoverSound(backButtonHoverSound);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "BACK";
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineRight;
        if (font != null) tmp.font = font;

        var trigger = go.AddComponent<EventTrigger>();
        var enterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enterEntry.callback.AddListener(_ => StartCoroutine(ScaleTo(rt, 1.2f, 0.1f)));
        trigger.triggers.Add(enterEntry);
        var exitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exitEntry.callback.AddListener(_ => StartCoroutine(ScaleTo(rt, 1f, 0.1f)));
        trigger.triggers.Add(exitEntry);
        var downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        downEntry.callback.AddListener(_ => StartCoroutine(ScaleTo(rt, 0.9f, 0.08f)));
        trigger.triggers.Add(downEntry);
        var upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        upEntry.callback.AddListener(_ => StartCoroutine(ScaleTo(rt, 1.2f, 0.08f)));
        trigger.triggers.Add(upEntry);
    }

    private RectTransform MakeRect(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }

    private void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    private IEnumerator ScaleTo(RectTransform rt, float target, float duration)
    {
        var start = rt.localScale.x;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / duration);
            var scale = Mathf.Lerp(start, target, t);
            rt.localScale = Vector3.one * scale;
            yield return null;
        }
        rt.localScale = Vector3.one * target;
    }
}
