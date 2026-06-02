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

        bool correct = false;
        switch (zoneType)
        {
            case ZoneType.A: correct = other.gameObject.name.Contains("Crate Short"); break;
            case ZoneType.B: correct = other.gameObject.name.Contains("Crate Long"); break;
            case ZoneType.C: correct = other.gameObject.name.Contains("Barrel"); break;
        }

        if (correct)
        {
            int earned = stats.gold;
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.AddGold(earned);
            Debug.Log($"[CollectZone {zoneType}] {other.gameObject.name} 올바른 구역! 골드 +{earned}");
        }
        else
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddDebt(50);
            Debug.Log($"[CollectZone {zoneType}] {other.gameObject.name} 잘못된 구역! -50 골드 패널티");
        }

        Destroy(other.gameObject);
    }
}