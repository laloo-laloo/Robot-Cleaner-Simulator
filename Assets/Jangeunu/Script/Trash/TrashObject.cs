using UnityEngine;

public class TrashObject : MonoBehaviour
{
    private enum TrashType
    {
        Dust,
        Liquid,
        Big
    }
    [SerializeField]
    private TrashType _trashType = TrashType.Dust;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerCleanManager playerClean = other.GetComponent<PlayerCleanManager>();
            if (playerClean != null)
            {
                PlayerCleanManager.CleaningMode currentMode = playerClean.Mode;
                ProcessSuckUp(currentMode);
            }
        }
    }

    private void ProcessSuckUp(PlayerCleanManager.CleaningMode mode)
    {
        if(mode == PlayerCleanManager.CleaningMode.Sweeping && _trashType == TrashType.Dust)
        {
            Debug.Log("쓸기");
            GameManager.Instance.AddCleanProgress();
            Destroy(gameObject);
        }
        else if(mode == PlayerCleanManager.CleaningMode.Wiping && _trashType == TrashType.Liquid)
        {
            Debug.Log("닦기");
            GameManager.Instance.AddCleanProgress();
            Destroy(gameObject);
        }
    }

    public bool CleaningTrash() //BaseStation 호출용
    {
        GameManager.Instance.AddCleanProgress();
        Destroy(gameObject);
        return true;
    }
}
