using UnityEngine;

public class TrashObject : MonoBehaviour
{
    private enum TrashType
    {
        Dust,
        Liquid,
        Big
    }

    [SerializeField] private TrashType _trashType = TrashType.Dust;

    // 이 쓰레기가 속한 구역 (Start 시 자동 스캔)
    [SerializeField] private ZoneArea.ZoneType _zoneType;

    private void Start()
    {
        // 쓰레기 위치를 감지해 현재 자기가 속한 ZoneArea의 ZoneType을 자동으로 가져옵니다.
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (Collider hit in hits)
        {
            ZoneArea zone = hit.GetComponent<ZoneArea>();
            if (zone != null)
            {
                _zoneType = zone.CurrentZoneType;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCleanManager playerClean = other.GetComponent<PlayerCleanManager>();
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (playerClean != null)
            {
                PlayerCleanManager.CleaningMode currentMode = playerClean.Mode;
                ProcessSuckUp(currentMode, player);
            }
        }
    }

    private void ProcessSuckUp(PlayerCleanManager.CleaningMode mode, PlayerStats player)
    {
        if (mode == PlayerCleanManager.CleaningMode.Sweeping && _trashType == TrashType.Dust)
        {
            if (player.DustVolume < player.DustMaxVolume)
            {
                Debug.Log("쓸기");
                // 구역 타입을 인자로 전달합니다!
                GameManager.Instance.AddCleanProgress(_zoneType);
                player.AddDust();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("먼지통 용량 부족");
            }
        }
        else if (mode == PlayerCleanManager.CleaningMode.Wiping && _trashType == TrashType.Liquid)
        {
            Debug.Log("닦기");
            // 구역 타입을 인자로 전달합니다!
            GameManager.Instance.AddCleanProgress(_zoneType);
            Destroy(gameObject);
        }
    }

    public bool CleaningTrash() // BaseStation 호출용
    {
        // 구역 타입을 인자로 전달합니다!
        GameManager.Instance.AddCleanProgress(_zoneType);
        Destroy(gameObject);
        return true;
    }
}