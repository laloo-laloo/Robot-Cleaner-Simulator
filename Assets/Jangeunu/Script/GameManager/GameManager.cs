using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private float _currentTime;

    [Header("Trash Info")]
    [SerializeField] private int _totalTrashCount;
    [SerializeField] private int _destoryTrashCount;

    [Header("UI References")]
    [SerializeField] private GameObject _clearPanel;
    [SerializeField] private TextMeshProUGUI _clearTimeResultText;
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PauseScript _pauseScript;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_clearPanel != null)
            _clearPanel.SetActive(false);

        // 클리어 판단을 위해 전체 쓰레기 수 카운트는 유지
        TrashObject[] allTrashes = FindObjectsByType<TrashObject>(FindObjectsSortMode.None);
        _totalTrashCount = allTrashes.Length;
    }

    private void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            OnClickTitle();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_pauseScript.gameObject.activeSelf)
            {
                _pauseScript.CloseUI();
            }
            else
            {
                _pauseScript.OpenUI();
            }
        }

        

        if (_destoryTrashCount >= _totalTrashCount && _totalTrashCount > 0)
        {
            GameClear();
        }
        else
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        _currentTime += Time.deltaTime;
        int minutes = (int)(_currentTime / 60);
        int seconds = (int)(_currentTime % 60);
        _timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void AddCleanProgress(ZoneArea.ZoneType zoneType)
    {
        if (_totalTrashCount > _destoryTrashCount)
        {
            _destoryTrashCount++; // 전체 파괴 카운트 증가 (클리어 조건용)

            _playerStats.AddGold(1);
            // 구역 관리자에게만 UI 갱신 요청!
            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnTrashCleaned(zoneType);
            }
        }
    }

    private void GameClear()
    {
        _clearPanel.SetActive(true);
        _clearTimeResultText.text = "클리어 시간: " + _timerText.text;

        PlayerPrefs.SetFloat("LatestScore", _currentTime);
        PlayerPrefs.Save();
    }

    public void OnClickTitle()
    {
        SceneManager.LoadScene("Title");
    }
}