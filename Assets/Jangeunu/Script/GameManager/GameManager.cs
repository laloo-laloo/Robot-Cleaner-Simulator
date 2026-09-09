using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Trash Info")]
    [SerializeField] private int _totalTrashCount;
    [SerializeField] private int _destoryTrashCount;

    [Header("UI References")]
    [SerializeField] private GameObject _clearPanel;
    [SerializeField] private TextMeshProUGUI _clearResultText;

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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                _pauseScript.CloseUI();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _pauseScript.OpenUI();
                Time.timeScale = 0f;
            }
        }

        

        if (_destoryTrashCount >= _totalTrashCount && _totalTrashCount > 0)
        {
            GameClear();
        }
    }

    public string GetTrashName(TrashObject.TrashType type)
    {
        return type switch
        {
            TrashObject.TrashType.Dust => "먼지",
            TrashObject.TrashType.Liquid => "얼룩",
            TrashObject.TrashType.Big => "큰 쓰레기",
            _ => "쓰레기"
        };
    }

    public Color GetTrashColor(TrashObject.TrashType type)
    {
        return type switch
        {
            TrashObject.TrashType.Dust => Color.yellow,                       // 노랑
            TrashObject.TrashType.Liquid => new Color(0.2f, 0.6f, 1f),       // 하늘색
            TrashObject.TrashType.Big => new Color(1f, 0.5f, 0f),            // 주황색
            _ => Color.white
        };
    }

    public void AddCleanProgress(ZoneArea.ZoneType zoneType, TrashObject.TrashType trashType)
    {
        if (_totalTrashCount > _destoryTrashCount)
        {
            _destoryTrashCount++;
            _playerStats.AddGold(1);

            // 1. 쓰레기 이름과 색상 꺼내기
            string trashName = GetTrashName(trashType);
            Color textColor = GetTrashColor(trashType);

            // 2. ZoneManager 호출 시 전달 (ZoneManager.OnTrashCleaned 메서드 매개변수도 동일하게 확장)
            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnTrashCleaned(zoneType, trashName, textColor);
            }
        }
    }

    private void GameClear()
    {
        _clearPanel.SetActive(true);
        _clearResultText.text = "클리어";

        PlayerPrefs.Save();
    }

    public void OnClickTitle()
    {
        SceneManager.LoadScene("Title");
    }
}