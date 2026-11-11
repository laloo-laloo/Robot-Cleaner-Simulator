using UnityEngine;

public class KitchenDeskItem : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            gameObject.tag = "Trash";
            gameObject.GetComponent<TrashObject>().enabled = true;
        }
    }
}
