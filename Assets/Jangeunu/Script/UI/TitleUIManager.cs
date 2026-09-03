using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _rankPanel;

    void Start()
    {
        _rankPanel.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnClickStart()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        SceneManager.LoadScene("v1.3.0");
    }

    public void OnClickOpenRank()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        _rankPanel.SetActive(true);
    }

    public void OnClickCloseRank()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        _rankPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        Application.Quit();
    }
}
