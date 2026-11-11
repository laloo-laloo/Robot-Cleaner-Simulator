using System.Collections;
using UnityEngine;

public class KitchenDesk : MonoBehaviour
{
    public void HitDesk(Vector3 direction, float force)
    {
        Debug.Log("Ãæµ¹");
        Rigidbody[] items = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in items)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }

    }
}
