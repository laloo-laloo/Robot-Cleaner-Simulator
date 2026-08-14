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
            Player player =  other.gameObject.GetComponentInParent<Player>();

            player._playerStats.RechargeBattery();
            player._playerStats.EmptyingDust();
            player.PlayerMoveStop();

            player.transform.position = transform.position;

            _upgradeUI.OpenUpgradeUI();
        }
        if (other.gameObject.CompareTag("Trash"))
        {
            TrashObject trash = other.GetComponent<TrashObject>();

            if (trash != null)
            {
                // 2. 해당 인스턴스의 CleaningTrash() 호출 (내부에서 GameManager 신호 전달 및 Destroy 처리됨)
                trash.CleaningTrash();
            }
        }
    }
}
