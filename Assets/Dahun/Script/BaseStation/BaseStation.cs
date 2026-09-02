using UnityEngine;

public class BaseStation : MonoBehaviour
{
    [SerializeField] private UpgradeUI _upgradeUI;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("충돌 감지");
        if (other.gameObject.CompareTag("Player"))
        {
            print("플레이어 충돌 감지");
            Player player = other.gameObject.GetComponentInParent<Player>();

            if (player != null)
            {
                player._playerStats.RechargeBattery();
                player._playerStats.EmptyingDust();
                player.PlayerMoveStop();

                player.transform.position = transform.position;

                // [추가] 플레이어 바구니에 담긴 큰 쓰레기 비우기
                ClearBasketTrash(player._playerStats);

                _upgradeUI.OpenUpgradeUI();
            }
        }
    }

    // 바구니 속 큰 쓰레기를 처리하는 메서드
    private void ClearBasketTrash(PlayerStats playerStats)
    {
        if (playerStats == null || playerStats.BasketObject == null) return;

        // 바구니 자식(Child)에 붙어있는 모든 TrashObject 탐색 (비활성화된 콜라이더도 포함하여 감지)
        TrashObject[] carriedTrashes = playerStats.BasketObject.GetComponentsInChildren<TrashObject>();

        foreach (TrashObject trash in carriedTrashes)
        {
            // CleaningTrash()를 호출해 GameManager 점수 추가 및 Destroy 처리
            trash.CleaningTrash();
        }
    }
}
