using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFxDemoLooper : MonoBehaviour
{
    public float interval = 2f;

    ParticleSystem ps;
    float elapsed;

    void Awake() { ps = GetComponent<ParticleSystem>(); }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed < interval) return;
        elapsed = 0f;
        ps.Clear(true);
        ps.Play(true);
    }
}
