using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public float dayDuration = 300f; // 5분
    public int baseQuota = 100;
    public int quotaIncrement = 50;

    [Header("Player")]
    public Transform xrOrigin;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    [Header("UI References")]
    public Image fadePanel;
    public Text dayText;
    public Text timerText;
    public Text quotaText;
    public Text debtText;

    public int currentDay = 1;
    public int currentQuota;
    public float timeRemaining;
    public bool isDayActive = false;

    private Coroutine _debtCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (xrOrigin != null)
        {
            _startPosition = xrOrigin.position;
            _startRotation = xrOrigin.rotation;
        }
        currentQuota = baseQuota;
        StartCoroutine(StartDay());
    }

    void Update()
    {
        if (!isDayActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isDayActive = false;
            StartCoroutine(EndDay());
        }

        UpdateTimerUI();
        UpdateQuotaUI();
    }

    IEnumerator StartDay()
    {
        // 검은 화면에서 일차 표시
        SetFade(1f);
        dayText.text = currentDay + "일차";
        dayText.gameObject.SetActive(true);
        debtText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        // 페이드 인
        yield return StartCoroutine(Fade(1f, 0f, 1.5f));
        dayText.gameObject.SetActive(false);

        timeRemaining = dayDuration;
        isDayActive = true;
    }

    IEnumerator EndDay()
    {
        // 페이드 아웃
        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        // 할당량 미달 체크
        int gold = PlayerStats.Instance != null ? PlayerStats.Instance.gold : 0;
        if (gold < currentQuota)
        {
            // Game Over
            ResetPlayerPosition();
            dayText.text = "Game Over";
            dayText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            dayText.gameObject.SetActive(false);

            // 1일차로 리셋
            currentDay = 1;
            currentQuota = baseQuota;
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.gold = 0;

            yield return StartCoroutine(StartDay());
        }

        ResetPlayerPosition();
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.gold = 0;
        currentDay++;
        currentQuota += quotaIncrement;

        dayText.text = currentDay + "일차";
        dayText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(Fade(1f, 0f, 1.5f));
        dayText.gameObject.SetActive(false);

        timeRemaining = dayDuration;
        isDayActive = true;
    }

    public void CheckQuota()
    {
        if (!isDayActive) return;
        if (PlayerStats.Instance == null) return;
        if (PlayerStats.Instance.gold >= currentQuota)
        {
            isDayActive = false;
            StartCoroutine(EndDay());
        }
    }

    public void AddDebt(int amount)
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.AddGold(-amount);

        if (_debtCoroutine != null) StopCoroutine(_debtCoroutine);
        _debtCoroutine = StartCoroutine(ShowDebt(amount));
    }

    IEnumerator ShowDebt(int amount)
    {
        debtText.text = "-" + amount + " 골드 (빚)";
        debtText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        debtText.gameObject.SetActive(false);
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.color = timeRemaining <= 60f ? Color.red : Color.white;
    }

    void UpdateQuotaUI()
    {
        int gold = PlayerStats.Instance != null ? PlayerStats.Instance.gold : 0;
        quotaText.text = "할당량: " + gold + " / " + currentQuota;
        quotaText.color = gold >= currentQuota ? new Color(0.2f, 0.9f, 0.3f) : Color.white;
    }

    void ResetPlayerPosition()
    {
        if (xrOrigin != null)
        {
            xrOrigin.position = _startPosition;
            xrOrigin.rotation = _startRotation;
        }
    }

    void SetFade(float alpha)
    {
        var c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetFade(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetFade(to);
    }
}