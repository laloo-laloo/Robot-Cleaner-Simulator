using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject _zoneUIPanel;
    [SerializeField] private TextMeshProUGUI _zoneNameText;
    [SerializeField] private TextMeshProUGUI _zoneProgressText;
    [SerializeField] private Slider _zoneProgressSlider;

    private Dictionary<ZoneArea.ZoneType, ZoneArea> _zoneDict = new Dictionary<ZoneArea.ZoneType, ZoneArea>();
    private ZoneArea.ZoneType _activeZoneType; // 현재 플레이어가 서 있는 방

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
        // 시작할 때는 구역 UI를 꺼둡니다.
        if (_zoneUIPanel != null)
            _zoneUIPanel.SetActive(false);
    }

    // 구역 등록
    public void RegisterZone(ZoneArea.ZoneType zoneType, ZoneArea zoneArea)
    {
        if (!_zoneDict.ContainsKey(zoneType))
        {
            _zoneDict.Add(zoneType, zoneArea);
        }
    }

    // 플레이어가 특정 방에 들어왔을 때
    public void OnPlayerEnterZone(ZoneArea.ZoneType zoneType)
    {
        _activeZoneType = zoneType;

        if (_zoneUIPanel != null)
            _zoneUIPanel.SetActive(true);

        UpdateZoneUI(zoneType);
    }

    // 특정 구역에서 쓰레기가 지워졌을 때 (GameManager에서 호출)
    public void OnTrashCleaned(ZoneArea.ZoneType zoneType)
    {
        if (_zoneDict.TryGetValue(zoneType, out ZoneArea targetZone))
        {
            targetZone.CleanOneTrash();

            // 현재 플레이어가 서 있는 방의 쓰레기가 지워졌다면 UI 즉시 갱신
            if (_activeZoneType == zoneType)
            {
                UpdateZoneUI(zoneType);
            }
        }
    }

    // 해당 구역의 청소율(%) 계산
    public float GetZoneCleanProgress(ZoneArea.ZoneType zoneType)
    {
        if (_zoneDict.TryGetValue(zoneType, out ZoneArea targetZone))
        {
            int total = targetZone.TotalTrashCount;
            int remaining = targetZone.CurrentTrashCount;

            if (total <= 0) return 100f;

            int cleaned = total - remaining;
            return ((float)cleaned / total) * 100f;
        }

        return 0f;
    }

    // UI 텍스트 갱신 함수
    private void UpdateZoneUI(ZoneArea.ZoneType zoneType)
    {
        float progress = GetZoneCleanProgress(zoneType);

        if (_zoneNameText != null)
            _zoneNameText.text = GetKoreanZoneName(zoneType);

        if (_zoneProgressText != null)
            _zoneProgressText.text = $"{progress:F1}%";

        if (_zoneProgressSlider != null)
            _zoneProgressSlider.value = progress / 100f;
    }

    // Enum을 한글 방 이름으로 변환
    private string GetKoreanZoneName(ZoneArea.ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneArea.ZoneType.LivingRoom: return "거실 청소율";
            case ZoneArea.ZoneType.Kitchen: return "주방 청소율";
            case ZoneArea.ZoneType.MasterRoom: return "안방 청소율";
            case ZoneArea.ZoneType.DrawingRoom: return "응접실 청소율";
            case ZoneArea.ZoneType.hallway1: return "복도1 청소율";
            default: return "구역";
        }
    }
}
