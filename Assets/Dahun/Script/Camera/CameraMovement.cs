using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform Player;

    [SerializeField] private Vector3 _offset = new Vector3(0, 1, -1.5f);
    [SerializeField] private float _freeLookSensitivity;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _wallAlpha = 0.25f;
    [SerializeField] private float _cameraRadius = 0.3f;

    private readonly Dictionary<Renderer, (Material mat, Color originalColor)> _fadedWalls = new();

    private float _freeYaw;
    public float FreeYaw => _freeYaw;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        _freeYaw = (_freeYaw + mouseDelta.x * _freeLookSensitivity) % 360f;

        Quaternion yawRotation = Quaternion.Euler(0f, _freeYaw, 0f);
        Vector3 desiredPosition = Player.position + yawRotation * _offset;
        transform.position = GetCollisionAdjustedPosition(desiredPosition);
        transform.rotation = yawRotation;
    }

    private Vector3 GetCollisionAdjustedPosition(Vector3 desiredPosition)
    {
        Vector3 direction = desiredPosition - Player.position;
        float distance = direction.magnitude;

        HashSet<Renderer> currentHits = new HashSet<Renderer>();

        // 경로상 벽 감지 (SphereCast는 여러 개를 한 번에 못 잡으니 CastAll 사용)
        RaycastHit[] pathHits = Physics.SphereCastAll(Player.position, _cameraRadius, direction.normalized, distance, _collisionMask);
        foreach (var h in pathHits)
        {
            Renderer r = h.collider.GetComponent<Renderer>();
            if (r != null) currentHits.Add(r);
        }

        // 카메라 도착 지점 자체가 벽에 파묻혔는지 추가 체크
        Collider[] overlaps = Physics.OverlapSphere(desiredPosition, _cameraRadius, _collisionMask);
        foreach (var col in overlaps)
        {
            Renderer r = col.GetComponent<Renderer>();
            if (r != null) currentHits.Add(r);
        }

        // 새로 닿은 것들은 투명하게
        foreach (var r in currentHits)
        {
            if (!_fadedWalls.ContainsKey(r))
            {
                SetWallTransparent(r);
            }
        }

        // 더 이상 안 닿는 것들은 복구
        List<Renderer> toRestore = new List<Renderer>();
        foreach (var r in _fadedWalls.Keys)
        {
            if (!currentHits.Contains(r))
            {
                toRestore.Add(r);
            }
        }
        foreach (var r in toRestore)
        {
            RestoreWall(r);
        }

        return desiredPosition;
    }

    private void SetupTransparentMode(Material mat)
    {
        mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private void SetupOpaqueMode(Material mat)
    {
        mat.SetFloat("_Surface", 0);
        mat.SetOverrideTag("RenderType", "Opaque");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
    }

    private void RestoreWall(Renderer targetRenderer)
    {
        if (!_fadedWalls.TryGetValue(targetRenderer, out var data)) return;

        data.mat.color = data.originalColor;
        SetupOpaqueMode(data.mat);

        _fadedWalls.Remove(targetRenderer);
    }

    private void SetWallTransparent(Renderer targetRenderer)
    {
        Material matInstance = targetRenderer.material; // 인스턴스화됨 (공유 머티리얼 안 건드림)
        Color originalColor = matInstance.color;

        SetupTransparentMode(matInstance);
        matInstance.color = new Color(originalColor.r, originalColor.g, originalColor.b, _wallAlpha);

        _fadedWalls[targetRenderer] = (matInstance, originalColor);
    }

    public void SetSensitivity(float value)
    {
        _freeLookSensitivity = value;
    }
}