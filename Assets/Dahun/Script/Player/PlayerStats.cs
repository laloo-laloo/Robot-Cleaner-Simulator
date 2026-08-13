using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _dustVolume = 0, _batteryVolume = 100;
    [SerializeField] private float _dustMaxVolume = 100, _batteryMaxVolume = 100;
    [SerializeField] private float _moveSpeed = 2;
    [SerializeField] private float _gold = 0;

    [SerializeField] private UpgradeUI _upgradeUI;

    private CapsuleCollider _capsuleCollider;
    private int[] statLevel = new int[4];
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
    public float Gold => _gold;
    public void AddGold(float amount)
    {
        _gold += amount;
    }
    public int GetStatLevel(StatType type) => statLevel[(int)type] + 1;
    public float GetUpgradeCost(StatType type)
    {
        int level = statLevel[(int)type];
        if (level >= Cost.GetLength(1)) return -1; // 최대 레벨이면 -1 등으로 표시
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
        
    }

    // Update is called once per frame
    void Update()
    {
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
        _capsuleCollider.radius = 0.7f;
    }

    public void Upgrade(StatType type, float amount)
    {
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
            case StatType.Battery: _batteryVolume += amount; break;
            case StatType.DustBin: _dustMaxVolume += amount; break;
        }
        _upgradeUI.UpdateUI();
    }

    public void MoveSpeedUp()
    {
        Upgrade(StatType.MoveSpeed, 1f);
    }
    public void BatteryUp()
    {
        Upgrade(StatType.Battery, 25f);
    }
    public void RangeUp()
    {
        Upgrade(StatType.Range, 0.2f);
    }
    public void DustBinUp()
    {
        Upgrade(StatType.DustBin, 25f);
    }
}
