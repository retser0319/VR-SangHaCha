using UnityEngine;
using UnityEngine.UI;

public class GoldHUD : MonoBehaviour
{
    private Text _goldText;

    void Start()
    {
        // 씬에서 GoldText 찾기
        var allTexts = GameObject.FindObjectsOfType<Text>(true);
        foreach (var t in allTexts)
        {
            if (t.gameObject.name == "GoldText")
            {
                _goldText = t;
                break;
            }
        }

        if (_goldText == null)
            Debug.LogError("[GoldHUD] GoldText를 찾을 수 없습니다!");
        else
            Debug.Log("[GoldHUD] GoldText 연결 완료");
    }

    void Update()
    {
        if (_goldText != null && PlayerStats.Instance != null)
            _goldText.text = "Gold: " + PlayerStats.Instance.gold;
    }
}