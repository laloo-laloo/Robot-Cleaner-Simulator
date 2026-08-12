using UnityEngine;

public class ZoneArea : MonoBehaviour
{
    public enum ZoneType
    {
        LivingRoom,
        Kitchen,
        MainRoom
    }
    [SerializeField] private ZoneType _zoneType;

    [SerializeField] private BoxCollider _zoneCollider;
    [SerializeField] private GameObject _zoneCleaningRatePanel;

    [SerializeField] private int _trashCountInZone = 0;

    public int TotalTrashCount => _trashCountInZone;
    public int CurrentTrashCount { get; private set; }

    void Start()
    {
        if (_zoneCollider == null)
        {
            _zoneCollider = GetComponent<BoxCollider>();
        }

        Vector3 zoneCenter = transform.TransformPoint(_zoneCollider.center);
        Vector3 zoneHalfExtents = Vector3.Scale(_zoneCollider.size, transform.lossyScale) * 0.5f;
        Quaternion zoneRotation = transform.rotation;

        Collider[] zoneHitColliders = Physics.OverlapBox(zoneCenter, zoneHalfExtents, zoneRotation);

        foreach (Collider col in zoneHitColliders)
        {
            if (col.GetComponent<TrashObject>() != null)
            {
                _trashCountInZone++;
            }
        }

        CurrentTrashCount = _trashCountInZone;

        ZoneManager.Instance.ReGisterZone(_zoneType, this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _zoneCleaningRatePanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _zoneCleaningRatePanel.SetActive(false);
        }
    }
}
