using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public int gold = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[PlayerStats] 골드 +{amount} 획득! 총 골드: {gold}");
    }
}