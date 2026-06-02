using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 화면의 씬 전환을 담당합니다.
/// 시작 버튼의 OnClick에 LoadGameScene()을 연결하세요.
/// </summary>
public class TitleMenuController : MonoBehaviour
{
    [Tooltip("게임 씬 이름 (Build Settings에 등록되어 있어야 함)")]
    [SerializeField]
    string m_GameSceneName = "BasicScene";

    /// <summary>
    /// 게임 씬으로 전환합니다. 시작 버튼에 연결하세요.
    /// </summary>
    public void LoadGameScene()
    {
        SceneManager.LoadScene(m_GameSceneName);
    }

    /// <summary>
    /// 게임을 종료합니다. (종료 버튼용)
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
