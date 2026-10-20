using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _stageSelectPanel;  // 비활성화 해두었던 스테이지 선택 탭
    [SerializeField] private GameObject _reportPanel;    //  버그 문의(건의사항) 패널 추가!

    private void Start()
    {
        // 게임 시작 시 스테이지 선택 탭은 확실히 꺼두고 시작
        if (_stageSelectPanel != null)
            _stageSelectPanel.SetActive(false);
        if (_reportPanel != null)
            _reportPanel.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnClickStart()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);

        // 2. 스테이지 선택 탭을 띄워줌!
        if (_stageSelectPanel != null)
            _stageSelectPanel.SetActive(true);
    }

    public void OnClickBackToMain()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);

        if (_stageSelectPanel != null)
            _stageSelectPanel.SetActive(false);
    }

    public void OnClickOpenReport()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        if (_reportPanel != null)
            _reportPanel.SetActive(true);
    }

    // 건의사항 패널 안의 [닫기] 버튼에 연결할 메서드
    public void OnClickCloseReport()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        if (_reportPanel != null)
            _reportPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        Application.Quit();
    }
}
