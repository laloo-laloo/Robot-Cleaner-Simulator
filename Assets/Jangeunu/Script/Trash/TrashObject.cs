using UnityEngine;

public class TrashObject : MonoBehaviour
{
    public enum TrashType
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

    [Header("Trash Effects")]
    [SerializeField] private GameObject _cleanDustTrashParticlePrefab;   // 먼지 흡입 시 터지는 이펙트
    [SerializeField] private GameObject _cleanLiquidParticlePrefab; // 액체 닦을 시 터지는 이펙트
    [SerializeField] private GameObject _cleanBigTrashParticlePrefab; // 큰 쓰레기 소멸 시 터질 파티클 프리팹

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

        // 바구니가 있고, 바구니 용량이 남아있는지 확인
        if (hasBasket && player.CanCarryMoreTrash)
        {
            // 바구니 위치로 부착 (이미 여러 개일 경우 위치 겹침 방지를 위해 살짝 위로 쌓이게 Offset 조절)
            int currentCount = player.CurrentCarryingTrashCount;
            Vector3 spawnOffset = new Vector3(0, 0.2f + (currentCount * 0.15f), 0);

            AttachToBasket(player.BasketObject.transform, spawnOffset);
        }
        else
        {
            // 바구니가 없거나 용량이 다 찼으면 튕겨나감
            BounceAway(playerTransform.position);
        }
    }

    private void AttachToBasket(Transform basketTransform, Vector3 offset)
    {
        if (_rb != null) _rb.isKinematic = true;
        if (_collider != null) _collider.enabled = false;

        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in childColliders)
        {
            col.enabled = false;
        }

        transform.SetParent(basketTransform);
        transform.localPosition = offset; // 개수에 따라 차곡차곡 쌓임
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
        // 1. 먼지(Dust) 청소
        if (mode == PlayerCleanManager.CleaningMode.Sweeping && _trashType == TrashType.Dust)
        {
            if (player.DustVolume < player.DustMaxVolume)
            {
                Debug.Log("쓸기");
                SoundManager.Instance.PlaySFX(SoundManager.SFX.SuckDust);
                GameManager.Instance.AddCleanProgress(_zoneType, _trashType);
                player.AddDust();

                SpawnEffect(_cleanDustTrashParticlePrefab);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("먼지통 용량 부족");
            }
        }
        // 2. 액체(Liquid) 청소
        else if (mode == PlayerCleanManager.CleaningMode.Wiping && _trashType == TrashType.Liquid)
        {
            Debug.Log("닦기");
            SoundManager.Instance.PlaySFX(SoundManager.SFX.WipeLipuid);
            GameManager.Instance.AddCleanProgress(_zoneType, _trashType);

            SpawnEffect(_cleanLiquidParticlePrefab);
            Destroy(gameObject);
        }
    }

    // 3. 큰 쓰레기(Big) 청소 (BaseStation에서 호출)
    public bool CleaningTrash()
    {
        SpawnEffect(_cleanBigTrashParticlePrefab);
        GameManager.Instance.AddCleanProgress(_zoneType, _trashType);
        Destroy(gameObject);
        return true;
    }

    private void SpawnEffect(GameObject particlePrefab)
    {
        if (particlePrefab == null) return;

        GameObject effect = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();

        float destroyDelay = (ps != null) ? ps.main.duration + ps.main.startLifetime.constantMax : 2.0f;
        Destroy(effect, destroyDelay);
    }
}