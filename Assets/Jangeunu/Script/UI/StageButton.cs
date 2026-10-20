using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
#if UNITY_EDITOR
    public SceneAsset stageSceneAsset;
#endif
    public string stageSceneName; // 런타임(빌드 후)에는 실제 씬 이름(string)을 사용

    private Button _button;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(OnClickStageButton);
        }
    }

    private void OnClickStageButton()
    {
        // 클릭 효과음 재생 (SoundManager가 있는 경우)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClick);
        }

        // 입력된 씬 이름으로 이동
        if (!string.IsNullOrEmpty(stageSceneName))
        {
            SceneManager.LoadScene(stageSceneName);
        }
        else
        {
            Debug.LogWarning("StageButton: 이동할 씬 이름이 설정되지 않았습니다!");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (stageSceneAsset != null)
        {
            stageSceneName = stageSceneAsset.name;
        }
    }
#endif
}
