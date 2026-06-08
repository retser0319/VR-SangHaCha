using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class GoldHUD : MonoBehaviour
{
    public static GoldHUD Instance { get; private set; }

    public Text _goldText;

    private TextMeshProUGUI _areaText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        var areaGo = GameObject.Find("AreaText");
        if (areaGo != null)
            _areaText = areaGo.GetComponent<TextMeshProUGUI>();

        if (_areaText != null)
            _areaText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_goldText != null && PlayerStats.Instance != null)
            _goldText.text = "Gold: " + PlayerStats.Instance.gold;
    }

    public void ShowAreaText(string zone)
    {
        if (_areaText == null) return;
        _areaText.text = "Area: " + zone;
        _areaText.gameObject.SetActive(true);
    }

    public void HideAreaText()
    {
        if (_areaText == null) return;
        _areaText.gameObject.SetActive(false);
    }
}