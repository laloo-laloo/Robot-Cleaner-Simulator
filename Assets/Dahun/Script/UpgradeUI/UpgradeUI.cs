using TMPro;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public PlayerStats PlayerStats;

    [SerializeField] private TMP_Text _moveSpeedStatCostText, _rangeStatCostText, _batteryStatCostText, _dustBinStatCostText, _currentGoldText;
    [SerializeField] private TMP_Text _moveSpeedStatLVText, _rangeStatLVText, _batteryStatLVText, _dustBinStatLVText;
    [SerializeField] private TMP_Text _moveSpeedAmountText, _rangeAmountText, _batteryAmountText, _dustBinAmountText;
    [SerializeField] private TMP_Text _basketUpgradeText, _basketLVText;

    private bool _isBasketUpgrade;

    private static readonly PlayerStats.StatType[] _statTypes =
    {
        PlayerStats.StatType.MoveSpeed,
        PlayerStats.StatType.Range,
        PlayerStats.StatType.Battery,
        PlayerStats.StatType.DustBin
    };

    private TMP_Text[] _lvTexts;
    private TMP_Text[] _costTexts;

    private void Awake()
    {
        _lvTexts = new[] { _moveSpeedStatLVText, _rangeStatLVText, _batteryStatLVText, _dustBinStatLVText };
        _costTexts = new[] { _moveSpeedStatCostText, _rangeStatCostText, _batteryStatCostText, _dustBinStatCostText };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _statTypes.Length; i++)
        {
            PlayerStats.StatType type = _statTypes[i];
            int level = PlayerStats.GetStatLevel(type);
            float cost = PlayerStats.GetUpgradeCost(type);

            _lvTexts[i].text = "LV." + level;
            _costTexts[i].text = cost == -1 ? "Max" : cost + "$";

            UpgradeAmountText(type);
        }

        _currentGoldText.text = PlayerStats.Gold + "$";
    }

    public void UpgradeAmountText(PlayerStats.StatType type)
    {
        switch (type)
        {
            case PlayerStats.StatType.Battery:
                _batteryAmountText.text = $"{PlayerStats.BatteryMaxVolume} -> {PlayerStats.BatteryMaxVolume + 25f}";
                break;
            case PlayerStats.StatType.DustBin:
                _dustBinAmountText.text = $"{PlayerStats.DustMaxVolume} -> {PlayerStats.DustMaxVolume + 25f}";
                break;
            case PlayerStats.StatType.MoveSpeed:
                _moveSpeedAmountText.text = $"{PlayerStats.MoveSpeed} -> {PlayerStats.MoveSpeed + 1f}";
                break;
            case PlayerStats.StatType.Range:
                _rangeAmountText.text = $"{PlayerStats.Range} -> {PlayerStats.Range + 0.2f}";
                break;
        }
    }

    public void OpenUpgradeUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.SetActive(true);
    }
    public void CloseUpgradeUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
    public void UpdateBasketPurchasButton()
    {
        if (!_isBasketUpgrade)
        {
            _basketUpgradeText.text = "Max";
            _basketLVText.text = "Owned";
            PlayerStats.BuyingBasket();
        }
    }
}
