using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _trashObject;
    [SerializeField] private Collider _spawnArea;

    private void OnEnable()
    {
        int randomCount = Random.Range(10, 19);
        for (int i = 0; i < randomCount; i++)
        {
            TrashSpawn();
        }
    }

    private void TrashSpawn()
    {
        Bounds bounds = _spawnArea.bounds;
        float randomValueX = Random.Range(bounds.min.x, bounds.max.x);
        float randomValueZ = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 randomPosition = new Vector3(randomValueX, 0.6f, randomValueZ);

        Instantiate(_trashObject, randomPosition, Quaternion.identity);
    }
}
