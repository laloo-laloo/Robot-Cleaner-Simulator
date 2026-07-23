using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private float _currentTime;

    [SerializeField] private int _totalTrashCount;
    [SerializeField] private int _destoryTrashCount;

    [SerializeField] private GameObject _clearPanel;
    [SerializeField] private TextMeshProUGUI _clearTimeResultText;

    [SerializeField] private TextMeshProUGUI _timerText;
    
    [SerializeField] private Slider _cleaningRateSlider;
    [SerializeField] private TextMeshProUGUI _cleaningRateText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _clearPanel.SetActive(false);
        TrashObject[] allTrashes = FindObjectsByType<TrashObject>(FindObjectsSortMode.None);
        _totalTrashCount = allTrashes.Length;
    }

    void Update()
    {
        if (_destoryTrashCount >= _totalTrashCount)
        {
            GameClear();
        }
        else
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        _currentTime += Time.deltaTime;
        int minutes = (int)(_currentTime / 60);
        int seconds = (int)(_currentTime % 60);
        string timeString = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        _timerText.text = timeString;
    }

    public void AddCleanProgress()
    {
        if (_totalTrashCount > _destoryTrashCount)
        {
            _destoryTrashCount++;
            float progressPercent = ((float)_destoryTrashCount / _totalTrashCount) * 100f;
            _cleaningRateText.text = "청소율 - " + progressPercent.ToString("F0") + "%";
            _cleaningRateSlider.value = (float)_destoryTrashCount / _totalTrashCount;
        }
    }

    private void GameClear()
    {
        _clearPanel.SetActive(true);
        _clearTimeResultText.text = "클리어 시간: " + _timerText.text;

        PlayerPrefs.SetFloat("LatestScore", _currentTime);
        PlayerPrefs.Save();
    }
}
