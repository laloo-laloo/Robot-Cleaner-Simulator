using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _dustVolume, _batteryVolume;
    [SerializeField] private float _dustMaxVolume, _batteryMaxVolume;
    [SerializeField] private float _moveSpeed;

    private CapsuleCollider _capsuleCollider;
    //private int[] level = { 1, 1, 1, 1 };
    //↑↑↑ 이거 업그레이드 몇번 했는지 기록하는거, 순서는 StatType이랑 똑같이

    public enum StatType { MoveSpeed, Range, Battery, DustBin };


    public float DustVolume => _dustVolume;
    public float DustMaxVolume => _dustMaxVolume;
    public float BatteryVolume => _batteryVolume;
    public float BatteryMaxVolume => _batteryMaxVolume;
    public float MoveSpeed => _moveSpeed;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        _dustMaxVolume = 100;
        _batteryMaxVolume = 100;
        _dustVolume = 0;
        _batteryVolume = _batteryMaxVolume;
        _moveSpeed = 1f;
        _capsuleCollider.radius = 0.7f;
    }

    public void Upgrade(StatType type, float amount)
    {
        switch (type)
        {
            case StatType.MoveSpeed: _moveSpeed += amount; break;
            case StatType.Range: _capsuleCollider.radius += amount; break;
            case StatType.Battery: _batteryVolume += amount; break;
            case StatType.DustBin: _dustMaxVolume += amount; break;
        }
    }

    public void MoveSpeedUp()
    {
        //만약에 살 돈이 된다면
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
