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

    [Header("Big Trash Settings")]
    [SerializeField] private float _bounceForce = 3f; // 바구니 없을 때 튕겨나가는 힘

    private Rigidbody _rb;
    private Collider _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

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

    // 1. 일반 쓰레기 (Dust, Liquid) - Trigger 상태일 때 작동
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 큰 쓰레기는 Trigger가 꺼져 있으므로 여기서 처리하지 않음
        if (_trashType == TrashType.Big) return;

        PlayerCleanManager playerClean = other.GetComponent<PlayerCleanManager>();
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (playerClean != null)
        {
            ProcessSuckUp(playerClean.Mode, player);
        }
    }

    // 2. 큰 쓰레기 (Big) - 일반 Collider 상태일 때 물리 충돌로 작동
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (_trashType == TrashType.Big)
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            // collision.collider 대신 플레이어의 Transform을 전달합니다.
            HandleBigTrashInteraction(collision.transform, player);
        }
    }

    private void HandleBigTrashInteraction(Transform playerTransform, PlayerStats player)
    {
        if (player == null) return;

        bool hasBasket = player.BasketObject != null && player.BasketObject.activeSelf;

        if (hasBasket)
        {
            // 최상위 부모(자신) 전체를 바구니에 부착
            AttachToBasket(player.BasketObject.transform);
        }
        else
        {
            // 튕겨나갈 때도 전체가 함께 이동
            BounceAway(playerTransform.position);
        }
    }

    private void AttachToBasket(Transform basketTransform)
    {
        // 1. 최상위 및 모든 자식의 물리/콜라이더 비활성화
        if (_rb != null) _rb.isKinematic = true;
        if (_collider != null) _collider.enabled = false;

        // 2. 자식들 중 콜라이더가 따로 있다면 모두 꺼줍니다.
        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in childColliders)
        {
            col.enabled = false;
        }

        // 3. 최상위 부모 오브젝트 자체를 바구니 자식으로 이동
        transform.SetParent(basketTransform);
        transform.localPosition = new Vector3(0, 0.2f, 0); // Y값 조절
        transform.localRotation = Quaternion.identity;
    }

    private void BounceAway(Vector3 playerPosition)
    {
        if (_rb == null) return;

        Vector3 pushDirection = (transform.position - playerPosition);
        pushDirection.y = 0;
        pushDirection = pushDirection.normalized;

        Vector3 forceVector = pushDirection + (Vector3.up * 0.5f);
        _rb.AddForce(forceVector * _bounceForce, ForceMode.Impulse);
    }

    private void ProcessSuckUp(PlayerCleanManager.CleaningMode mode, PlayerStats player)
    {
        if (mode == PlayerCleanManager.CleaningMode.Sweeping && _trashType == TrashType.Dust)
        {
            if (player.DustVolume < player.DustMaxVolume)
            {
                Debug.Log("쓸기");
                SoundManager.Instance.PlaySFX(SoundManager.SFX.SuckDust);
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
            SoundManager.Instance.PlaySFX(SoundManager.SFX.WipeLipuid);
            GameManager.Instance.AddCleanProgress(_zoneType);
            Destroy(gameObject);
        }
    }

    public bool CleaningTrash() // 큰 쓰레기 치우기 위한 BaseStation 호출용
    {
        GameManager.Instance.AddCleanProgress(_zoneType);
        Destroy(gameObject);
        return true;
    }
}