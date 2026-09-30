using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float _dustVolume, _batteryVolume;
    private float _dustMaxVolume, _batteryMaxVolume;
    private PlayerController _playerController;
    public PlayerStats _playerStats;

    private Color _defaultColor;

    private Coroutine _blinkCoroutineA;
    private Coroutine _blinkCoroutineB;

    public float DustVolume => _dustVolume;
    public float BatteryVolume => _batteryVolume;
    public float DustMaxVolume => _dustMaxVolume;
    public float BatteryMaxVolume => _batteryMaxVolume;


    [SerializeField] private Slider _dustSlider, _batterySlider;


    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#A8C080", out _defaultColor);

        _playerStats = GetComponent<PlayerStats >();
        _playerController = GetComponent<PlayerController>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        SliderValueUpdate();
        _dustMaxVolume = _playerStats.DustMaxVolume;
        _batteryMaxVolume = _playerStats.BatteryMaxVolume;
    }

    private void Initialize()
    {
        _dustVolume = 0;
        _batteryVolume = 100;
        _dustSlider.value = _dustVolume;
        _batterySlider.value = _batteryVolume;
    }

    public void SliderValueUpdate()
    {
        _dustVolume = _playerStats.DustVolume;
        _dustSlider.value = _dustVolume / _playerStats.DustMaxVolume * 100;
        _batteryVolume = _playerStats.BatteryVolume;
        _batterySlider.value = _batteryVolume / _batteryMaxVolume * 100;
    }

    
    public float CheckDustValue() => _dustSlider.value;
    public float CheckBatteryValue() => _batterySlider.value;
    

    public void PlayerParkingStation()
    {
        _playerController.PlayerMoveStop();
    }

    private IEnumerator BlinkSlider(Slider slider)
    {
        Image fillImage = slider.fillRect.GetComponent<Image>();

        while (true)
        {
            fillImage.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            fillImage.color = Color.white;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void StartBlinkDust()
    {
        if (_blinkCoroutineA != null) return;
        _blinkCoroutineA = StartCoroutine(BlinkSlider(_dustSlider));
    }

    public void StopBlinkDust()
    {
        if (_blinkCoroutineA == null) return;
        StopCoroutine(_blinkCoroutineA);
        _blinkCoroutineA = null;
        _dustSlider.fillRect.GetComponent<Image>().color = _defaultColor;
    }
    public void StartBlinkBattery()
    {
        if (_blinkCoroutineB != null) return;
        _blinkCoroutineB = StartCoroutine(BlinkSlider(_batterySlider));
    }

    public void StopBlinkBattery()
    {
        if (_blinkCoroutineB == null) return;
        StopCoroutine(_blinkCoroutineB);
        _blinkCoroutineB = null;
        _batterySlider.fillRect.GetComponent<Image>().color = _defaultColor;
    }
}
