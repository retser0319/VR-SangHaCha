using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// 걷기/달리기 + 계단 오르기 기능을 지원하는 플레이어 이동 컴포넌트.
/// 멀티 레이캐스트로 계단 끝 턱까지 감지해서 올라갑니다.
/// </summary>
public class PlayerMoveProvider : DynamicMoveProvider
{
    [Header("Player Movement Settings")]
    [Tooltip("걷기 속도 (m/s)")]
    [SerializeField, Range(0.5f, 20f)]
    float m_WalkSpeed = 7.5f;

    [Tooltip("달리기 속도 (m/s)")]
    [SerializeField, Range(0.5f, 20f)]
    float m_RunSpeed = 17.5f;

    [Tooltip("달리기 전환 InputAction (예: 조이스틱 클릭, Shift 키 등)")]
    [SerializeField]
    InputActionProperty m_RunAction;

    [Header("Stair Climbing Settings")]
    [Tooltip("계단으로 인식할 최대 높이 (m)")]
    [SerializeField, Range(0f, 1f)]
    float m_StairHeight = 0.45f;

    [Tooltip("계단 감지 앞 거리 (m)")]
    [SerializeField, Range(0.1f, 1f)]
    float m_StairCheckDistance = 0.35f;

    [Tooltip("계단 오를 때 Y축 이동 속도")]
    [SerializeField, Range(1f, 20f)]
    float m_StairClimbSpeed = 10f;

    [Tooltip("계단 감지 레이어 마스크")]
    [SerializeField]
    LayerMask m_StairLayerMask = ~0;

    bool m_IsRunning;
    CharacterController m_CharacterController;

    public bool IsRunning => m_IsRunning;
    public float CurrentSpeed => m_IsRunning ? m_RunSpeed : m_WalkSpeed;

    protected override void Awake()
    {
        moveSpeed = m_WalkSpeed;
        base.Awake();
        m_CharacterController = GetComponentInParent<CharacterController>();
        m_RunAction.action?.Enable();
    }

    void OnDestroy()
    {
        m_RunAction.action?.Disable();
    }

    protected new void Update()
    {
        // 부모(ContinuousMoveProvider)의 실제 이동 로직을 먼저 실행해야 리그가 움직인다.
        UpdateRunState();
        moveSpeed = m_IsRunning ? m_RunSpeed : m_WalkSpeed;
        base.Update();
        HandleStairClimbing();
    }

    void UpdateRunState()
    {
        bool runPressed = m_RunAction.action != null && m_RunAction.action.IsPressed();
        if (runPressed == m_IsRunning) return;
        m_IsRunning = runPressed;
    }

    void HandleStairClimbing()
    {
        if (m_CharacterController == null) return;

        // 이동 중일 때만 감지
        Vector3 horizontalVelocity = new Vector3(m_CharacterController.velocity.x, 0, m_CharacterController.velocity.z);
        if (horizontalVelocity.magnitude < 0.1f) return;

        Vector3 moveDir = horizontalVelocity.normalized;
        Vector3 origin = m_CharacterController.transform.position;

        // 여러 높이에서 레이캐스트를 쏴서 계단 감지
        // (0.05, 0.15, 0.25, 0.35 높이) — 마지막 턱도 감지하기 위해 높이 촘촘히
        float[] checkHeights = { 0.05f, 0.15f, 0.25f, 0.35f };

        foreach (float checkHeight in checkHeights)
        {
            Vector3 rayOrigin = origin + Vector3.up * checkHeight;

            // 앞에 장애물 있는지 확인
            if (!Physics.Raycast(rayOrigin, moveDir, m_StairCheckDistance, m_StairLayerMask))
                continue;

            // 장애물 위에서 아래로 레이 쏴서 올라설 수 있는 높이 확인
            Vector3 aboveOrigin = origin + moveDir * m_StairCheckDistance + Vector3.up * m_StairHeight;
            if (!Physics.Raycast(aboveOrigin, Vector3.down, out RaycastHit hitInfo, m_StairHeight + 0.05f, m_StairLayerMask))
                continue;

            float stepHeight = hitInfo.point.y - origin.y;
            if (stepHeight <= 0 || stepHeight > m_StairHeight) continue;

            // Y축 강제 이동
            m_CharacterController.Move(Vector3.up * stepHeight * Time.deltaTime * m_StairClimbSpeed);
            break; // 한 번만 처리
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        moveSpeed = m_IsRunning ? m_RunSpeed : m_WalkSpeed;
    }
#endif
}
