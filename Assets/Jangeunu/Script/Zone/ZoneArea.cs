using System.Collections.Generic;
using UnityEngine;

public class ZoneArea : MonoBehaviour
{
    public enum ZoneType
    {
        LivingRoom,  // 거실
        Kitchen,     // 주방
        MasterRoom,  // 안방
        DrawingRoom, // 응접실
        Hallway1,    // 복도1
        Hallway2,    // 복도2
        SpareRoom,   // 작은방
    }

    [Header("Zone Settings")]
    [SerializeField] private ZoneType _zoneType;

    [SerializeField] private BoxCollider[] _zoneColliders;

    public ZoneType CurrentZoneType => _zoneType;
    public int TotalTrashCount { get; private set; }
    public int CurrentTrashCount { get; private set; }

    private void Start()
    {
        if (_zoneColliders == null || _zoneColliders.Length == 0)
        {
            _zoneColliders = GetComponents<BoxCollider>();
        }

        HashSet<TrashObject> uniqueTrashes = new HashSet<TrashObject>();

        foreach (BoxCollider col in _zoneColliders)
        {
            if (col == null) continue;

            Vector3 zoneCenter = col.transform.TransformPoint(col.center);
            Vector3 zoneHalfExtents = Vector3.Scale(col.size, col.transform.lossyScale) * 0.5f;
            Quaternion zoneRotation = col.transform.rotation;

            Collider[] zoneHitColliders = Physics.OverlapBox(zoneCenter, zoneHalfExtents, zoneRotation);

            foreach (Collider hit in zoneHitColliders)
            {
                TrashObject trash = hit.GetComponent<TrashObject>();
                if (trash != null)
                {
                    uniqueTrashes.Add(trash);
                }
            }
        }

        // [수정 포인트 1] 단순 개수(Count)가 아닌 가중치(GetTrashWeight)의 합산으로 Total 계산
        int totalWeight = 0;
        foreach (TrashObject trash in uniqueTrashes)
        {
            totalWeight += trash.GetTrashWeight();
        }

        TotalTrashCount = totalWeight;
        CurrentTrashCount = TotalTrashCount;

        if (ZoneManager.Instance != null)
        {
            ZoneManager.Instance.RegisterZone(_zoneType, this);
        }
    }

    // [수정 포인트 2] 프로퍼티인 CurrentTrashCount를 직접 차감하도록 수정
    public void CleanTrash(int weight)
    {
        CurrentTrashCount = Mathf.Max(0, CurrentTrashCount - weight);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ZoneManager.Instance?.OnPlayerEnterZone(_zoneType);
        }
    }
}