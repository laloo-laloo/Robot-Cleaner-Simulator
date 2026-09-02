using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform Player;

    [SerializeField] private Vector3 _offset = new Vector3(0, 1, -1.5f);
    [SerializeField] private float _freeLookSensitivity;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _wallAlpha = 0.25f;
    [SerializeField] private float _cameraRadius = 0.2f;
    //[SerializeField] private float _fadeSpeed = 8f;
    private Renderer _fadedRenderer;
    private Material _fadedMaterialInstance;
    private Color _originalColor;

    private float _freeYaw;
    public float FreeYaw => _freeYaw;

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

        Debug.Log(_freeLookSensitivity);
    }

    private Vector3 GetCollisionAdjustedPosition(Vector3 desiredPosition)
    {
        Vector3 direction = desiredPosition - Player.position;
        float distance = direction.magnitude;

        if (Physics.SphereCast(Player.position, _cameraRadius, direction.normalized, out RaycastHit hit, distance, _collisionMask))
        {
            SetWallTransparent(hit.collider.GetComponent<Renderer>());
        }
        else
        {
            RestoreWall();
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

    private void RestoreWall()
    {
        if (_fadedMaterialInstance == null) return;

        _fadedMaterialInstance.color = _originalColor;
        SetupOpaqueMode(_fadedMaterialInstance);

        _fadedRenderer = null;
        _fadedMaterialInstance = null;
    }

    private void SetWallTransparent(Renderer targetRenderer)
    {
        if (_fadedRenderer == targetRenderer) return;

        RestoreWall();

        _fadedRenderer = targetRenderer;
        _fadedMaterialInstance = targetRenderer.material; // 인스턴스화됨 (공유 머티리얼 안 건드림)
        _originalColor = _fadedMaterialInstance.color;

        SetupTransparentMode(_fadedMaterialInstance);
        _fadedMaterialInstance.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _wallAlpha);
    }

    public void SetSensitivity(float value)
    {
        _freeLookSensitivity = value;
    }
}