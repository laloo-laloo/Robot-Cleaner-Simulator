using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void OnClickStart()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        SceneManager.LoadScene("v1.5.0");
    }

    public void OnClickExit()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        Application.Quit();
    }
}
