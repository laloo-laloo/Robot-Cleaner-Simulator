using UnityEngine;

public class VacuumVortexController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _vortexParticle;
    [SerializeField] private PlayerStats _playerStats;

    private ParticleSystem.EmissionModule _emissionModule;
    private ParticleSystem.ShapeModule _shapeModule;

    // 기본 Collider 반지름(0.7f) 대비 Shape Module의 기본 Radius 비율 계산용
    private const float BASE_COLLIDER_RADIUS = 0.7f;
    private const float BASE_VORTEX_RADIUS = 1.15f;

    private void Awake()
    {
        if (_vortexParticle != null)
        {
            _emissionModule = _vortexParticle.emission;
            _shapeModule = _vortexParticle.shape;
        }
    }

    private void Update()
    {
        UpdateVortexScaleByRange();
    }

    private void UpdateVortexScaleByRange()
    {
        if (_playerStats == null || _vortexParticle == null) return;

        // 콜라이더 크기 비율에 맞춰 파티클 발생 도넛 반지름 확장
        float currentRadius = _playerStats.Range;
        float radiusRatio = currentRadius / BASE_COLLIDER_RADIUS;

        _shapeModule.radius = BASE_VORTEX_RADIUS * radiusRatio;
    }
}
