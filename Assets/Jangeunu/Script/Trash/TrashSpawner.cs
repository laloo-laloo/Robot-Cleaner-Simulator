using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    // 단일 오브젝트 대신 쓰레기 프리팹 배열 선언
    [SerializeField] private GameObject[] _trashObjects;
    [SerializeField] private Collider _spawnArea;

    private void OnEnable()
    {
        // 배열이 비어있는지 검사
        if (_trashObjects == null || _trashObjects.Length == 0)
        {
            Debug.LogWarning("TrashSpawner: 등록된 쓰레기 프리팹이 없습니다.");
            return;
        }

        int randomCount = Random.Range(70, 99); //소환할 쓰레기 개수 랜덤 설정
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
        Vector3 randomPosition = new Vector3(randomValueX, 0.515f, randomValueZ);

        // 프리팹 배열 중 무작위 인덱스 선택
        int randomIndex = Random.Range(0, _trashObjects.Length);
        GameObject selectedTrash = _trashObjects[randomIndex];

        // 회전값도 Y축 무작위로 적용.
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Instantiate(selectedTrash, randomPosition, randomRotation);
    }
}