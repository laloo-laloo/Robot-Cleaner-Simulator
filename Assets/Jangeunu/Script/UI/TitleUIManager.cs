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
        SceneManager.LoadScene("Test");
    }

    public void OnClickOpenRank()
    {
        _rankPanel.SetActive(true);
    }

    public void OnClickCloseRank()
    {
        _rankPanel.SetActive(false);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
}
