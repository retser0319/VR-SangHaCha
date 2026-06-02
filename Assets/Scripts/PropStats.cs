using UnityEngine;

public class PropStats : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int gold;

    private float _spawnTime;
    private const float SpawnGracePeriod = 1.0f; // 스폰 후 1초간 충돌 무시

    void Awake()
    {
        _spawnTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 스폰 직후 유예 시간
        if (Time.time - _spawnTime < SpawnGracePeriod)
            return;

        // 바닥 충돌 무시
        if (collision.gameObject.CompareTag("Floor"))
            return;

        // 충돌 강도가 약하면 무시
        if (collision.relativeVelocity.magnitude < 1.0f)
            return;

        int prevHealth = health;
        health = Mathf.Max(0, health - 1);

        float ratio = (float)(prevHealth - health) / maxHealth;
        int goldLost = Mathf.RoundToInt(gold * ratio);
        gold = Mathf.Max(0, gold - goldLost);

        Debug.Log($"[PropStats] {gameObject.name} 충돌 - 체력: {prevHealth} -> {health}, 골드: {gold} (-{goldLost})");

        if (health <= 0)
        {
            gold = 0;
            Debug.Log($"[PropStats] {gameObject.name} 파괴! 골드 0");
        }
    }
}