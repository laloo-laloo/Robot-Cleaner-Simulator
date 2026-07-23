using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private float _dustVolume, _batteryVolume;
    private float _dustMaxVolume, _batteryMaxVolume;
    private PlayerController _playerController;


    [SerializeField] private Slider _dustSlider, _batterySlider;


    private void Awake()
    {
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
        CunsumeBattery();
    }

    private void Initialize()
    {
        _dustMaxVolume = 100;
        _batteryMaxVolume = 100;
        _dustVolume = 0;
        _batteryVolume = _batteryMaxVolume;
        _dustSlider.value = _dustVolume;
        _batterySlider.value = _batteryVolume;
    }

    public void EmptyingDust()
    {
        StartCoroutine(DecreaseDust());
    }

    public void RechargeBattery()
    {
        StartCoroutine(Recharging());
    }

    private void CunsumeBattery()
    {
        _batteryVolume = Mathf.MoveTowards(_batteryVolume, 0f, 0.2f * Time.deltaTime);
        _batterySlider.value = _batteryVolume;
    }

    public void AddDust()
    {
        _dustVolume += 1;
        _dustSlider.value = _dustVolume;
    }

    private IEnumerator DecreaseDust()
    {
        while (_dustVolume > 0)
        {
            _dustVolume = Mathf.MoveTowards(_dustVolume, 0f, 50 * Time.deltaTime);
            _dustSlider.value = _dustVolume;
            yield return null;
        }
    }
    private IEnumerator Recharging()
    {
        while (_batteryVolume < 99.9f)
        {
            _batteryVolume = Mathf.MoveTowards(_batteryVolume, 100f, 50f * Time.deltaTime);
            _batterySlider.value = _batteryVolume;
            yield return null;
        }
    }

    public float CheckDustValue()
    {
        return _dustSlider.value;
    }

    public float CheckBatteryValue()
    {
        return _batterySlider.value;
    }

    public void PlayerMoveStop()
    {
        _playerController.PlayerMoveStop();
    }
}
