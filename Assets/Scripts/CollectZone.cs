using UnityEngine;

public class CollectZone : MonoBehaviour
{
    public enum ZoneType { A, B, C }
    public ZoneType zoneType;

    void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponent<PropStats>();
        if (stats == null) stats = other.GetComponentInParent<PropStats>();
        if (stats == null) return;

        bool correct = stats.assignedZone == zoneType.ToString();

        if (correct)
        {
            int earned = stats.gold;
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.AddGold(earned);
            Debug.Log($"[CollectZone {zoneType}] {other.gameObject.name} 정답! 골드 +{earned}");
        }
        else
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddDebt(50);
            Debug.Log($"[CollectZone {zoneType}] {other.gameObject.name} 오답! (배정={stats.assignedZone}) -50 패널티");
        }

        Destroy(other.gameObject);
    }
}