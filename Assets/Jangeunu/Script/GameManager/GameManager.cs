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

        // 단순 개수가 아닌 '가중치 합산'으로 TotalTrashCount 계산
        TrashObject[] allTrashes = FindObjectsByType<TrashObject>(FindObjectsSortMode.None);
        _totalTrashCount = 0;

        foreach (var trash in allTrashes)
        {
            _totalTrashCount += trash.GetTrashWeight(); // Big 쓰레기는 30으로 카운트됨
        }
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

    public void AddCleanProgress(ZoneArea.ZoneType zoneType, TrashObject.TrashType trashType, int weight = 1, int gold = 1)
    {
        if (_totalTrashCount > _destoryTrashCount)
        {
            // 1. 파괴 카운트(청소율)는 weight 만큼 증가
            _destoryTrashCount += weight;

            // 2. 플레이어 골드는 별도의 gold 매개변수 값만큼만 증가
            if (_playerStats != null)
            {
                _playerStats.AddGold(gold); // weight가 아닌 gold 할당!
            }

            string trashName = GetTrashName(trashType);
            Color textColor = GetTrashColor(trashType);

            if (ZoneManager.Instance != null)
            {
                ZoneManager.Instance.OnTrashCleaned(zoneType, trashName, textColor, weight);
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