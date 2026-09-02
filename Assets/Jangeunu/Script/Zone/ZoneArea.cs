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

    // 1. 단일 변수 대신 배열로 변경
    [SerializeField] private BoxCollider[] _zoneColliders;

    public ZoneType CurrentZoneType => _zoneType;
    public int TotalTrashCount { get; private set; }
    public int CurrentTrashCount { get; private set; }

    private void Start()
    {
        // Inspector에서 할당을 잊었을 경우 자식/본인의 BoxCollider를 자동 탐색
        if (_zoneColliders == null || _zoneColliders.Length == 0)
        {
            _zoneColliders = GetComponents<BoxCollider>();
        }

        // 중복 쓰레기 수집을 방지하기 위한 HashSet
        HashSet<TrashObject> uniqueTrashes = new HashSet<TrashObject>();

        // 2. 등록된 모든 BoxCollider 영역을 스캔
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
                    uniqueTrashes.Add(trash); // 이미 추가된 쓰레기면 자동 중복 제외
                }
            }
        }

        TotalTrashCount = uniqueTrashes.Count;
        CurrentTrashCount = TotalTrashCount;

        // 3. ZoneManager에 등록
        if (ZoneManager.Instance != null)
        {
            ZoneManager.Instance.RegisterZone(_zoneType, this);
        }
    }

    public void CleanOneTrash()
    {
        if (CurrentTrashCount > 0)
        {
            CurrentTrashCount--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ZoneManager.Instance?.OnPlayerEnterZone(_zoneType);
        }
    }
}