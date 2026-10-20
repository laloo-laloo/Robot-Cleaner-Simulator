using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private UnityEditor.SceneAsset _sceneAsset; // 인스펙터에서 씬을 드래그 앤 드롭할 슬롯!
    [SerializeField] private string _sceneName; // 이동할 실제 유니티 씬 이름 (예: "Stage1", "Stage2")

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
        if (!string.IsNullOrEmpty(_sceneName))
        {
            SceneManager.LoadScene(_sceneName);
        }
        else
        {
            Debug.LogWarning("StageButton: 이동할 씬 이름이 설정되지 않았습니다!");
        }
    }

    private void OnValidate()
    {
        if (_sceneAsset != null)
        {
            _sceneName = _sceneAsset.name;
        }
    }
}
