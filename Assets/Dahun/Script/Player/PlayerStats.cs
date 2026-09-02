using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _dustVolume = 0, _batteryVolume = 100;
    [SerializeField] private float _dustMaxVolume = 100, _batteryMaxVolume = 100;
    [SerializeField] private float _moveSpeed = 2;
    [SerializeField] private float _gold = 0;
    [SerializeField] private float _currentSpeed;

    [SerializeField] private UpgradeUI _upgradeUI;
    [SerializeField] private ParticleSystem _moveSpeedFlashEffect, _rangeFlashEffect, _batteryFlashEffect, _dustBinFlashEffect;

    [SerializeField] private GameObject _moveSpeedMaxObject, _rangeMaxObject, _batteryMaxObject, _dustBinMaxObject;
    [SerializeField] private GameObject _basket;

    private CapsuleCollider _capsuleCollider;
    private int[] statLevel = new int[4];
    //속도, 범위, 배터리, 용량
    public float[,] Cost = { { 10, 20, 30, 40 },
                                { 10, 20, 30, 40 },
                                { 10, 20, 30, 40 },
                                { 10, 20, 30, 40 } };

    public enum StatType { MoveSpeed, Range, Battery, DustBin };


    public float DustVolume => _dustVolume;
    public float DustMaxVolume => _dustMaxVolume;
    public float BatteryVolume => _batteryVolume;
    public float BatteryMaxVolume => _batteryMaxVolume;
    public float MoveSpeed => _moveSpeed;
    public float Range => _capsuleCollider.radius;
    public float CurrentSpeed => _currentSpeed;
    public float Gold => _gold;
    public void AddGold(float amount)
    {
        _gold += amount;
    }
    public int GetStatLevel(StatType type) => statLevel[(int)type] + 1;
    public float GetUpgradeCost(StatType type)
    {
        int level = statLevel[(int)type];
        if (level >= Cost.GetLength(1)) return -1;
        return Cost[(int)type, level];
    }

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        Init();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveSpeedMaxObject.SetActive(false);
        _rangeMaxObject.SetActive(false);
        _batteryMaxObject.SetActive(false);
        _dustBinMaxObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CunsumeBattery();
        if (Keyboard.current.eKey.isPressed)
        {
            _gold += 100;
        }
    }

    private void Init()
    {
        _dustMaxVolume = 100;
        _batteryMaxVolume = 100;
        _dustVolume = 0;
        _batteryVolume = _batteryMaxVolume;
        _moveSpeed = 2f;
        _currentSpeed = _moveSpeed;
        _capsuleCollider.radius = 0.7f;
    }

    public void AddDust()
    {
        _dustVolume += 1;
    }

    public void Upgrade(StatType type, float amount)
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.Upgrade);
        int index = (int)type;
        int level = statLevel[index];

        if (level >= Cost.GetLength(1)) return;

        float cost = Cost[index, level];

        if (_gold < cost) return;

        _gold -= cost;
        statLevel[index]++;

        switch (type)
        {
            case StatType.MoveSpeed: _moveSpeed += amount; break;
            case StatType.Range: _capsuleCollider.radius += amount; break;
            case StatType.Battery: _batteryMaxVolume += amount; break;
            case StatType.DustBin: _dustMaxVolume += amount; break;
        }
        _upgradeUI.UpdateUI();
        _upgradeUI.UpgradeAmountText(type);
        PlayUpgradeFlash(type);

        CheckMaxLevel(type);
    }

    private void PlayUpgradeFlash(StatType type)
    {
        ParticleSystem effect = type switch
        {
            StatType.MoveSpeed => _moveSpeedFlashEffect,
            StatType.Range => _rangeFlashEffect,
            StatType.Battery => _batteryFlashEffect,
            StatType.DustBin => _dustBinFlashEffect,
            _ => null
        };

        if (effect == null) return;

        effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        effect.Play();
    }

    public void MoveSpeedUp()
    {
        Upgrade(StatType.MoveSpeed, 0.5f);
    }
    public void BatteryUp()
    {
        Upgrade(StatType.Battery, 10f);
    }
    public void RangeUp()
    {
        Upgrade(StatType.Range, 0.2f);
    }
    public void DustBinUp()
    {
        Upgrade(StatType.DustBin, 10f);
    }

    public void EmptyingDust()
    {
        StartCoroutine(DecreaseDust());
    }
    public void RechargeBattery()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.RechargingBattery);
        StartCoroutine(Recharging());
    }

    private IEnumerator DecreaseDust()
    {
        while (_dustVolume > 0)
        {
            _dustVolume = Mathf.MoveTowards(_dustVolume, 0f, 50 * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator Recharging()
    {
        while (_batteryVolume < (_batteryMaxVolume - 0.1f))
        {
            _batteryVolume = Mathf.MoveTowards(_batteryVolume, _batteryMaxVolume, 50f * Time.deltaTime);
            yield return null;
        }
    }
    private void CunsumeBattery()
    {
        if (_batteryVolume <= 0)
            _currentSpeed = 1f;
        else
            _currentSpeed = _moveSpeed;

        _batteryVolume = Mathf.MoveTowards(_batteryVolume, 0f, 0.8f * Time.deltaTime);
    }

    private void CheckMaxLevel(StatType type)
    {
        int index = (int)type;
        bool isMax = statLevel[index] >= Cost.GetLength(1);

        switch (type)
        {
            case StatType.MoveSpeed: _moveSpeedMaxObject.SetActive(isMax); break;
            case StatType.Range: _rangeMaxObject.SetActive(isMax); break;
            case StatType.Battery: _batteryMaxObject.SetActive(isMax); break;
            case StatType.DustBin: _dustBinMaxObject.SetActive(isMax); break;
        }
    }
    public void BuyingBasket()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.Upgrade);
        if (_gold >= 100f)
        {
            _gold -= 100;
            _basket.SetActive(true);
        }
    }
}
