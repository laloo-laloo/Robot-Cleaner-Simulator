using UnityEngine;

public class BaseStation : MonoBehaviour
{




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

            player.RechargeBattery();
            player.EmptyingDust();
            player.PlayerMoveStop();

            player.transform.position = transform.position;
        }
        if (other.gameObject.CompareTag("Trash"))
        {
            GameManager.Instance.AddCleanProgress();
            Destroy(other.gameObject);
        }
    }
}
