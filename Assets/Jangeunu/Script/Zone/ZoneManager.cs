using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    private Dictionary<ZoneArea.ZoneType, ZoneArea> _zoneDict = new Dictionary<ZoneArea.ZoneType, ZoneArea>();

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ReGisterZone(ZoneArea.ZoneType zoneType, ZoneArea zoneArea)
    {
        if (!_zoneDict.ContainsKey(zoneType))
        {
            _zoneDict.Add(zoneType, zoneArea);
        }
    }

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
}
