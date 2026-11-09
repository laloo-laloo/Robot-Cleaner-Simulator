using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _dustVolume = 0, _batteryVolume = 100;
    [SerializeField] private float _dustMaxVolume = 100, _batteryMaxVolume = 100;
    [SerializeField] private float _moveSpeed = 2;
    [SerializeField] private float _currentSpeed;

    private float _cunsumeSpeed = 0.8f;

    [SerializeField] private Slider _goldSlider;
    [SerializeField] private float _gold;

    [SerializeField] private UpgradeUI _upgradeUI;
    [SerializeField] private ParticleSystem _moveSpeedFlashEffect, _rangeFlashEffect, _batteryFlashEffect, _dustBinFlashEffect, _basketFlashEffect;

    [SerializeField] private GameObject _moveSpeedMaxObject, _rangeMaxObject, _batteryMaxObject, _dustBinMaxObject;


    [SerializeField] private GameObject[] _moveSpeedLevelObjects;
    [SerializeField] private GameObject[] _rangeLevelObjects;
    [SerializeField] private GameObject[] _batteryLevelObjects;
    [SerializeField] private GameObject[] _dustBinLevelObjects;

    [SerializeField] private TMP_Text __goldText;

    [SerializeField] private GameObject _basket;
    [SerializeField] private int _basketMaxCapacity = 1; // 기본 최대 용량
    
    public GameObject GuideArrow;
    public int BasketMaxCapacity => _basketMaxCapacity;

    private bool _firstSkill = false, _secondSkill = false, _thirdSkill = false, _forthSkill = false;
    private bool _enableDecrease = true;

    // 현재 바구니에 담긴 큰 쓰레기 개수 반환
    public int CurrentCarryingTrashCount
    {
        get
        {
            if (_basket == null) return 0;
            return _basket.GetComponentsInChildren<TrashObject>().Length;
        }
    }

    // 바구니에 더 담을 수 있는지 확인
    public bool CanCarryMoreTrash => CurrentCarryingTrashCount < _basketMaxCapacity;
    public GameObject BasketObject => _basket;

    private CapsuleCollider _capsuleCollider;
    private int[] statLevel = new int[4];
    // 범위, 베터리, 속도, 용량
    public float[,] Cost =
    {
        { 40,  60,  80, 100 },
        { 30,  50,  70,  90 },
        { 50,  70,  90, 110 },
        { 30,  50,  70,  90 }
    };

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
        _gold = Mathf.Round(_gold * 100f) / 100f;
        updateGold();
    }
    public int GetStatLevel(StatType type) => statLevel[(int)type] + 1;
    public float GetUpgradeCost(StatType type)
    {
        int level = statLevel[(int)type];
        if (level >= Cost.GetLength(1)) return -1;
        return Cost[(int)type, level];
    }

    private void updateGold()
    {
        __goldText.text = Gold + "$";
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
        PressSkill();
        if (Keyboard.current.eKey.isPressed)
        {
            _gold += 100;
        }

        Transform target = GetClosestBaseStation();
        if (target == null) return;

        Vector3 dir = target.position - GuideArrow.transform.position;
        dir.y = 0; // 수평 방향만 가리키게
        GuideArrow.transform.rotation = Quaternion.LookRotation(dir);
    }

    private void PressSkill()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame && _firstSkill)
        {
            StartCoroutine(UseSkill(StatType.Range));
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame && _secondSkill)
        {
            StartCoroutine(UseSkill(StatType.Battery));
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame && _forthSkill)
        {
            //용량
        }
        if (Keyboard.current.leftShiftKey.isPressed && _thirdSkill)
        {
            //속도
            _currentSpeed = _moveSpeed * 2;
            _cunsumeSpeed = 1.6f;
        }
        else
        {
            //속도
            _currentSpeed = _moveSpeed;
            _cunsumeSpeed = 0.8f;
        }
    }
    

    private Transform GetClosestBaseStation()
    {
        BaseStation[] stations = FindObjectsByType<BaseStation>(FindObjectsSortMode.None);
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (var station in stations)
        {
            float distance = Vector3.Distance(transform.position, station.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = station.transform;
            }
        }

        return closest;
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
        _cunsumeSpeed = 0.8f;
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
        UpdateAppearance(type);
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

    public void UpgradeMoveSpeed()
    {
        Upgrade(StatType.MoveSpeed, 0.5f);
    }
    public void UpgradeBattery()
    {
        Upgrade(StatType.Battery, 10f);
    }
    public void UpgradeRange()
    {
        Upgrade(StatType.Range, 0.2f);
    }
    public void UpgradeDustBin()
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
    private IEnumerator UseSkill(StatType type)
    {
        bool isRangeCoolTime = false;
        switch (type)
        {
            case StatType.Range:
                //범위
                if (!isRangeCoolTime)
                {
                    if (BatteryVolume >= BatteryMaxVolume * 0.1f)
                    {
                        if (_enableDecrease)
                            _batteryVolume -= BatteryMaxVolume * 0.1f;
                        
                        float currentRadius = Range;
                        _capsuleCollider.radius = 5f;
                        yield return new WaitForSeconds(0.2f);
                        _capsuleCollider.radius = currentRadius;
                        isRangeCoolTime = true;
                    }
                }
                else
                {
                    float rangeCoolTime = 5;
                    if (isRangeCoolTime)
                    {
                        float currentCoolTime = rangeCoolTime;
                        if(currentCoolTime <= 0)
                        {
                            isRangeCoolTime = false;
                        }
                        else
                        {
                            currentCoolTime -= Time.deltaTime;
                        }
                    }
                }
                break;
            case StatType.Battery:
                if (BatteryVolume >= BatteryMaxVolume * 0.3f)
                {
                    _enableDecrease = false;
                    yield return new WaitForSeconds(30f);
                    _batteryVolume -= BatteryMaxVolume * 0.3f;
                    _enableDecrease = true;
                }
                break;
            case StatType.MoveSpeed:

                break;
            case StatType.DustBin:

                break;
        }
        yield return null;
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
        if (_enableDecrease)
            _batteryVolume = Mathf.MoveTowards(_batteryVolume, 0f, 0.8f * Time.deltaTime);
    }

    private void CheckMaxLevel(StatType type)
    {
        int index = (int)type;
        bool isMax = statLevel[index] >= Cost.GetLength(1);

        switch (type)
        {
            case StatType.Range: 
                _rangeMaxObject.SetActive(isMax);
                _firstSkill = isMax;
                break;
            case StatType.Battery: 
                _batteryMaxObject.SetActive(isMax); 
                _secondSkill = isMax;
                break;
            case StatType.MoveSpeed: 
                _moveSpeedMaxObject.SetActive(isMax); 
                _thirdSkill = isMax;
                break;
            case StatType.DustBin: 
                _dustBinMaxObject.SetActive(isMax); 
                _forthSkill = isMax;
                break;
        }
    }
    private void UpdateAppearance(StatType type)
    {
        int index = (int)type;
        int level = statLevel[index];

        GameObject[] targetObjects = type switch
        {
            StatType.MoveSpeed => _moveSpeedLevelObjects,
            StatType.Range => _rangeLevelObjects,
            StatType.Battery => _batteryLevelObjects,
            StatType.DustBin => _dustBinLevelObjects,
            _ => null
        };

        if (targetObjects == null) return;

        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].SetActive(i == level);
        }
    }

    public void BuyingBasket()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.Upgrade);
        if (_gold >= 100f)
        {
            _gold -= 100;
            if (_basketFlashEffect != null)
            {
                _basketFlashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _basketFlashEffect.Play();
            }
            
            _basket.SetActive(true);
        }
    }
}
