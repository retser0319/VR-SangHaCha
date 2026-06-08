using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 화면의 단계별 튜토리얼 페이지를 관리합니다.
/// 여러 페이지(GameObject)를 순서대로 보여주고,
/// NEXT 버튼으로 다음 페이지로 넘어갑니다. 마지막 페이지의
/// 시작 버튼은 게임 씬으로 전환합니다.
/// </summary>
public class TutorialFlowController : MonoBehaviour
{
    [Tooltip("순서대로 표시할 페이지들. 첫 페이지(0)만 처음에 켜지고 나머지는 꺼집니다.")]
    [SerializeField]
    GameObject[] m_Pages;

    [Tooltip("게임 씬 이름 (Build Settings에 등록되어 있어야 함)")]
    [SerializeField]
    string m_GameSceneName = "BasicScene";

    int m_CurrentIndex = 0;

    void Start()
    {
        ShowPage(0);
    }

    /// <summary>지정한 인덱스의 페이지만 활성화합니다.</summary>
    void ShowPage(int index)
    {
        for (int i = 0; i < m_Pages.Length; i++)
        {
            if (m_Pages[i] != null)
                m_Pages[i].SetActive(i == index);
        }
        m_CurrentIndex = index;
    }

    /// <summary>다음 페이지로 넘어갑니다. NEXT 버튼에 연결하세요.</summary>
    public void NextPage()
    {
        int next = m_CurrentIndex + 1;
        if (next < m_Pages.Length)
            ShowPage(next);
    }

    /// <summary>이전 페이지로 돌아갑니다. (BACK 버튼용, 선택)</summary>
    public void PreviousPage()
    {
        int prev = m_CurrentIndex - 1;
        if (prev >= 0)
            ShowPage(prev);
    }

    /// <summary>게임 씬으로 전환합니다. 마지막 시작 버튼에 연결하세요.</summary>
    public void LoadGameScene()
    {
        SceneManager.LoadScene(m_GameSceneName);
    }
}
