using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class PropStats : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public int gold;
    public string assignedZone; // "A", "B", "C"

    private float _spawnTime;
    private const float SpawnGracePeriod = 1.0f;

    private Renderer[] _renderers;
    private Color[] _originalColors;
    private Coroutine _flashCoroutine;

    void Awake()
    {
        _spawnTime = Time.time;

        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].material.color;

        var grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (GoldHUD.Instance != null)
            GoldHUD.Instance.ShowAreaText(assignedZone);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (GoldHUD.Instance != null)
            GoldHUD.Instance.HideAreaText();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time - _spawnTime < SpawnGracePeriod)
            return;

        if (collision.relativeVelocity.magnitude < 1.0f)
            return;

        int prevHealth = health;
        health = Mathf.Max(0, health - 1);

        float ratio = (float)(prevHealth - health) / maxHealth;
        int goldLost = Mathf.RoundToInt(gold * ratio);
        gold = Mathf.Max(0, gold - goldLost);

        Debug.Log($"[PropStats] {gameObject.name} 충돌 - 체력: {prevHealth} -> {health}, 골드: {gold} (-{goldLost})");

        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashRed());

        if (health <= 0)
        {
            gold = 0;
            Debug.Log($"[PropStats] {gameObject.name} 파괴!");
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _originalColors[i];

        _flashCoroutine = null;
    }
}