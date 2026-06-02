using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// 걷기/달리기 기능을 지원하는 플레이어 이동 컴포넌트.
/// 부모(DynamicMoveProvider)의 moveSpeed 프로퍼티를 직접 제어합니다.
///
/// [사용법]
/// XR Origin > Locomotion > Move 오브젝트에 추가하세요.
/// Inspector의 Walk Speed / Run Speed를 사용하세요. (부모의 Move Speed는 무시됩니다)
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

    [Header("Stair Settings")]
    [Tooltip("계단을 오를 수 있는 최대 높이. CharacterController의 Step Offset과 동일하게 설정하세요.")]
    [SerializeField, Range(0f, 0.5f)]
    float m_StepOffset = 0.3f;

    bool m_IsRunning;
    CharacterController m_CharacterController;

    public bool IsRunning => m_IsRunning;
    public float CurrentSpeed => m_IsRunning ? m_RunSpeed : m_WalkSpeed;

    protected override void Awake()
    {
        // moveSpeed를 base.Awake() 이전에 미리 설정해서
        // 부모가 초기화할 때 이미 우리 값이 들어가 있도록 합니다.
        moveSpeed = m_WalkSpeed;
        base.Awake();

        m_CharacterController = GetComponentInParent<CharacterController>();
        m_RunAction.action?.Enable();
    }

    void OnDestroy()
    {
        m_RunAction.action?.Disable();
    }

    void Update()
    {
        UpdateRunState();
        SyncStepOffset();
    }

    void UpdateRunState()
    {
        bool runPressed = m_RunAction.action != null && m_RunAction.action.IsPressed();
        if (runPressed == m_IsRunning) return;
        m_IsRunning = runPressed;
        moveSpeed = m_IsRunning ? m_RunSpeed : m_WalkSpeed;
    }

    void SyncStepOffset()
    {
        if (m_CharacterController != null)
            m_CharacterController.stepOffset = m_StepOffset;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        moveSpeed = m_IsRunning ? m_RunSpeed : m_WalkSpeed;
    }
#endif
}
