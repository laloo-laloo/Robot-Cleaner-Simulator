using TMPro;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public PlayerStats PlayerStats;
    private int _moveStatLevel, _rangeStatLevel, _batteryStatLevel, _dustBinStatLevel;

    [SerializeField] private TMP_Text _moveStatCostText, _rangeStatCostText, _batteryStatCostText, _dustBinStatCostText, _currentGoldText;
    [SerializeField] private TMP_Text _moveStatLVText, _rangeStatLVText, _batteryStatLVText, _dustBinStatLVText;

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
        _moveStatLevel = PlayerStats.GetStatLevel(PlayerStats.StatType.MoveSpeed);
        _rangeStatLevel = PlayerStats.GetStatLevel(PlayerStats.StatType.Range);
        _batteryStatLevel = PlayerStats.GetStatLevel(PlayerStats.StatType.Battery);
        _dustBinStatLevel = PlayerStats.GetStatLevel(PlayerStats.StatType.DustBin);

        float moveCost = PlayerStats.GetUpgradeCost(PlayerStats.StatType.MoveSpeed);
        _moveStatLVText.text = "LV." + _moveStatLevel;
        if (moveCost == -1)
            _moveStatLVText.text = "Max";
        else
            _moveStatCostText.text = moveCost + "$";

        float rangeCost = PlayerStats.GetUpgradeCost(PlayerStats.StatType.Range);
        _rangeStatLVText.text = "LV." + _rangeStatLevel;
        if (moveCost == -1)
            _rangeStatCostText.text = "Max";
        else
            _rangeStatCostText.text = rangeCost + "$";

        float batteryCost = PlayerStats.GetUpgradeCost(PlayerStats.StatType.Battery);
        _batteryStatLVText.text = "LV." + _batteryStatLevel;
        if (moveCost == -1)
            _batteryStatCostText.text = "Max";
        else
            _batteryStatCostText.text = batteryCost + "$";

        float dustBinCost = PlayerStats.GetUpgradeCost(PlayerStats.StatType.DustBin);
        _dustBinStatLVText.text = "LV." + _dustBinStatLevel;
        if (moveCost == -1)
            _dustBinStatCostText.text = "Max";
        else
            _dustBinStatCostText.text = dustBinCost + "$";

        _currentGoldText.text = PlayerStats.Gold + "$";
    }

    public void OpenUpgradeUI()
    {
        gameObject.SetActive(true);
    }
    public void CloseUpgradeUI()
    {
        gameObject.SetActive(false);
    }
}
