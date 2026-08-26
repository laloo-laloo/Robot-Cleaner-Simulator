using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    [SerializeField] private GameObject _settingPanel, _pausePanel, _audioPanel, _videoPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        CloseSetting();
    }
    public void OpenUI()
    {
        gameObject.SetActive(true);
    }
    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
    public void OpenSetting()
    {
        _settingPanel.SetActive(true);
        _pausePanel.SetActive(false);
    }
    public void CloseSetting()
    {
        _settingPanel.SetActive(false);
        _pausePanel.SetActive(true);
    }
    public void OpenPauseUI()
    {
        _pausePanel.SetActive(true);
    }
    public void ClosePauseUI()
    {
        _pausePanel.SetActive(false);
    }
    public void OpenVideo()
    {
        _videoPanel.SetActive(true);
        _audioPanel.SetActive(false);
    }
    public void OpenAudio()
    {
        _videoPanel.SetActive(false);
        _audioPanel.SetActive(true);
    }
    public void Exit()
    {
        GameManager.Instance.OnClickTitle();
    }
}
