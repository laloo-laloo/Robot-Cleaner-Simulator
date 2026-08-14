using UnityEngine;

public class ZoneArea : MonoBehaviour
{
    public enum ZoneType
    {
        LivingRoom,  // 거실
        Kitchen,     // 주방
        MasterRoom,     // 안방
        DrawingRoom,    // 응접실
        hallway1,   // 복도1

    }

    [Header("Zone Settings")]
    [SerializeField] private ZoneType _zoneType;
    [SerializeField] private BoxCollider _zoneCollider;

    // 외부에서 읽을 수 있도록 열어두는 프로퍼티
    public ZoneType CurrentZoneType => _zoneType;
    public int TotalTrashCount { get; private set; }
    public int CurrentTrashCount { get; private set; }

    private void Start()
    {
        // 1. 콜라이더 자동 할당
        BoxCollider[] zoneColliders = GetComponents<BoxCollider>();

        int count = 0;

        // 2. OverlapBox로 내 영역 안에 있는 쓰레기 스캔
        foreach (BoxCollider col in zoneColliders)
        {
            Vector3 zoneCenter = transform.TransformPoint(_zoneCollider.center);
            Vector3 zoneHalfExtents = Vector3.Scale(_zoneCollider.size, transform.lossyScale) * 0.5f;
            Quaternion zoneRotation = transform.rotation;

            Collider[] zoneHitColliders = Physics.OverlapBox(zoneCenter, zoneHalfExtents, zoneRotation);

            foreach (Collider hit in zoneHitColliders)
            {
                if (hit.GetComponent<TrashObject>() != null)
                {
                    count++;
                }
            }
        }

        TotalTrashCount = count;
        CurrentTrashCount = count;

        // 3. ZoneManager에 자기 자신 등록
        if (ZoneManager.Instance != null)
        {
            ZoneManager.Instance.RegisterZone(_zoneType, this);
        }
    }

    // 쓰레기가 하나 치워졌을 때 호출되는 함수
    public void CleanOneTrash()
    {
        if (CurrentTrashCount > 0)
        {
            CurrentTrashCount--;
        }
    }

    // 플레이어가 진입했을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ZoneManager에게 내가 속한 방으로 UI를 전환해 달라고 요청
            ZoneManager.Instance.OnPlayerEnterZone(_zoneType);
        }
    }
}