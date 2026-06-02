using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShelfBoxSpawner : MonoBehaviour
{
    [Header("Interactable")]
    [Tooltip("이 Shelf의 XRSimpleInteractable을 여기에 드래그하세요")]
    public XRSimpleInteractable interactable;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset = new Vector3(0f, 0.15f, -1.5f);
    public GameObject[] spawnPrefabs;

    private int _lastSpawnFrame = -1;
    private float _cooldownUntil = 0f;
    private const float CooldownSeconds = 30f;

    void Awake()
    {
        if (interactable == null)
        {
            Debug.LogError("[ShelfSpawner] " + gameObject.name + ": interactable 필드가 비어 있습니다!");
            return;
        }
        interactable.selectEntered.RemoveAllListeners();
        interactable.selectEntered.AddListener(OnTriggered);
    }

    void OnTriggered(SelectEnterEventArgs args)
    {
        if (Time.frameCount == _lastSpawnFrame) return;
        _lastSpawnFrame = Time.frameCount;

        if (Time.time < _cooldownUntil)
        {
            float remaining = _cooldownUntil - Time.time;
            Debug.Log($"[ShelfSpawner] {gameObject.name} 쿨타임 중 ({remaining:F1}초 남음)");
            return;
        }

        _cooldownUntil = Time.time + CooldownSeconds;
        SpawnProp();
    }

    void SpawnProp()
    {
        if (spawnPrefabs == null || spawnPrefabs.Length == 0)
        {
            Debug.LogError("[ShelfSpawner] " + gameObject.name + ": spawnPrefabs가 비어 있습니다!");
            return;
        }

        var prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        Vector3 worldOffset = transform.TransformDirection(spawnOffset);
        Vector3 spawnPos = transform.position + worldOffset;
        spawnPos.y = 1.0f;

        var obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // MeshCollider는 convex=true 설정 (Rigidbody와 호환)
        foreach (var mc in obj.GetComponentsInChildren<MeshCollider>())
            mc.convex = true;

        // Rigidbody 없으면 추가
        var rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;

        // XRGrabInteractable 없으면 추가
        var grab = obj.GetComponent<XRGrabInteractable>();
        if (grab == null)
            grab = obj.AddComponent<XRGrabInteractable>();
        grab.useDynamicAttach = true;
        grab.throwOnDetach = true;

        // PropStats 부착 및 스탯 설정
        var stats = obj.GetComponent<PropStats>();
        if (stats == null)
            stats = obj.AddComponent<PropStats>();

        switch (prefab.name)
        {
            case "Crate Short":
                stats.maxHealth = Random.Range(10, 21);
                stats.gold = 20;
                break;
            case "Crate Long":
                stats.maxHealth = Random.Range(10, 16);
                stats.gold = 30;
                break;
            case "Barrel":
                stats.maxHealth = Random.Range(5, 11);
                stats.gold = 50;
                break;
            default:
                stats.maxHealth = 10;
                stats.gold = 10;
                break;
        }
        stats.health = stats.maxHealth;

        Debug.Log($"[ShelfSpawner] {prefab.name} 스폰 완료: pos={spawnPos}, 체력={stats.health}, 골드={stats.gold}");
    }
}