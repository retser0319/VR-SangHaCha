using UnityEngine;

/// <summary>
/// 이 트리거 존에 PropStats가 붙은 오브젝트가 진입하면
/// 골드를 플레이어에게 주고 오브젝트를 제거합니다.
/// </summary>
public class GoldCollectZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var stats = other.GetComponent<PropStats>();
        if (stats == null)
            stats = other.GetComponentInParent<PropStats>();
        if (stats == null) return;

        int earned = stats.gold;

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.AddGold(earned);

        Debug.Log($"[GoldZone] {other.gameObject.name} 수집! 골드 +{earned}");
        Destroy(other.gameObject);
    }
}