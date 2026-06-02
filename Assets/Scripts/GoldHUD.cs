using UnityEngine;
using UnityEngine.UI;

public class GoldHUD : MonoBehaviour
{
    private Text _goldText;

    void Start()
    {
        // Canvas 생성
        var canvasGO = new GameObject("HUD Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // 배경 패널 (좌측 상단)
        var panelGO = new GameObject("GoldPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelRT = panelGO.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 1);
        panelRT.anchorMax = new Vector2(0, 1);
        panelRT.pivot = new Vector2(0, 1);
        panelRT.anchoredPosition = new Vector2(20, -20);
        panelRT.sizeDelta = new Vector2(200, 60);
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.6f);

        // 텍스트
        var textGO = new GameObject("GoldText");
        textGO.transform.SetParent(panelGO.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(10, 5);
        textRT.offsetMax = new Vector2(-10, -5);
        _goldText = textGO.AddComponent<Text>();
        _goldText.text = "Gold: 0";
        _goldText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _goldText.fontSize = 28;
        _goldText.fontStyle = FontStyle.Bold;
        _goldText.color = new Color(1f, 0.85f, 0.1f);
        _goldText.alignment = TextAnchor.MiddleLeft;
    }

    void Update()
    {
        if (_goldText != null && PlayerStats.Instance != null)
            _goldText.text = "Gold: " + PlayerStats.Instance.gold;
    }
}