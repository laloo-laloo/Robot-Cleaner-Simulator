using UnityEngine;

public class RoboticVacuumCleaner : MonoBehaviour
{
    [Header("Wheels")]
    public Transform leftWheel;
    public Transform rightWheel;
    public float driveWheelRadius = 0.072f;     // drive wheel radius [m]
    public float trackWidth = 0.115f;           // distance between drive wheels [m]
    public Transform frontPivot;
    public Transform frontWheel;
    public float casterWheelRadius = 0.0155f;   // front wheel radius [m]

    [Header("Brush, Rollers and Mops")]
    public Transform brush;                     // spinning brush disc mesh
    public float brushRPM = 100f;               // target RPM at full speed
    public float brushSpinEasing = 2f;          // seconds to ease in/out spin animation
    public bool brushEnabled = false;           // toggle spin/stop
    public Transform mopR;                      // spinning right mop disc mesh
    public Transform mopL;                      // spinning left mop disc mesh
    public float mopRPM = 100f;                 // target RPM at full speed
    public float mopSpinEasing = 3f;            // seconds to ease in/out spin animation
    public bool mopsEnabled = false;            // toggle spin/stop
    public Transform[] rollers;                 // spinning roller cylinder meshes
    public float rollerRPM = 100f;              // target RPM at full speed
    public float rollerSpinEasing = 3f;         // seconds to ease in/out spin animation
    public bool rollersEnabled = false;         // toggle spin/stop
    public Transform cap;                       // top lidar cap mesh
    public float capOffset = 0.011f;            // top lidar cap extended offset [m]
    public float capEasing = 2f;                // seconds to transition in/out lidar cap
    private Vector3 capStartingPosition;        // lidar cap default local position
    private Material material;                  // material reference
    private float currentMatEmission;           // current emission intensity
    private Color baseEmissionColor = Color.azure;

    [Header("Movement")]
    public float maxSpeed = 0.5f;               // linear velocity per wheel [m/s]
    public float movePower = 0.5f;              // input scaling

    [Header("Input axes")]
    public string verticalAxis = "Vertical";
    public string horizontalAxis = "Horizontal";

    private Rigidbody rb;
    private Transform cachedTransform;
    private const float RPM_TO_DEG_PER_SEC = 6f;
    private const float SAFE_MIN_EASING = 0.000001f;

    // store latest kinematic velocities so caster can use them
    private float linearVel;
    private float angularVel;

    // spinners
    private float currentBrushRPM = 0f; 
    private float targetBrushRPM = 0f;
    private float currentMopRPM = 0f;
    private float targetMopRPM = 0f;
    private float currentRollerRPM = 0f;
    private float targetRollerRPM = 0f;

    void Start()
    {
        cachedTransform = transform;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 20f;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (cap != null)
            capStartingPosition = cap.localPosition;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            material = renderer.material;
        else
            material = null;

        if (material != null && material.HasProperty("_EmissionColor"))
        {
            Color orig = material.GetColor("_EmissionColor");
            float maxChannel = Mathf.Max(orig.r, Mathf.Max(orig.g, orig.b));
            if (maxChannel > 0f)
            {
                baseEmissionColor = orig / maxChannel;
                currentMatEmission = 0f;
            }
            else
            {
                if (material.HasProperty("_Color"))
                {
                    Color albedo = material.GetColor("_Color");
                    float maxAlbedo = Mathf.Max(albedo.r, Mathf.Max(albedo.g, albedo.b));
                    if (maxAlbedo > 0f)
                        baseEmissionColor = albedo / maxAlbedo;
                    else
                        baseEmissionColor = Color.white;
                }
                else
                {
                    baseEmissionColor = Color.white;
                }
                currentMatEmission = 0f;
            }

            if (currentMatEmission > 0f)
                material.EnableKeyword("_EMISSION");
            else
                material.DisableKeyword("_EMISSION");
        }
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // --- INPUT ---
        float forward = Input.GetAxis(verticalAxis);
        float turn = Input.GetAxis(horizontalAxis);

        // --- DIFFERENTIAL DRIVE KINEMATICS ---
        float v = forward * movePower; // commanded linear velocity
        float w = turn * movePower;    // commanded angular velocity

        float halfTrack = trackWidth * 0.5f;

        float vLeft = v - (w * halfTrack);
        float vRight = v + (w * halfTrack);

        vLeft = Mathf.Clamp(vLeft, -maxSpeed, maxSpeed);
        vRight = Mathf.Clamp(vRight, -maxSpeed, maxSpeed);

        linearVel = (vRight + vLeft) * 0.5f;          // forward speed [m/s]
        angularVel = (vRight - vLeft) / trackWidth;   // yaw rate [rad/s]

        // --- APPLY TO BODY ---
        Vector3 forwardMove = cachedTransform.forward * linearVel * dt;
        float yawDeg = angularVel * Mathf.Rad2Deg * dt;
        Quaternion rotation = Quaternion.Euler(0f, yawDeg, 0f);

        rb.MovePosition(rb.position + forwardMove);
        rb.MoveRotation(rb.rotation * rotation);

        // --- DRIVE WHEEL SPIN ---
        RotateWheelMesh(leftWheel, vLeft, driveWheelRadius, dt);
        RotateWheelMesh(rightWheel, vRight, driveWheelRadius, dt);

        // --- CASTER WHEEL ---
        UpdateCaster(frontPivot, frontWheel, dt);

        // --- LIDAR CAP ---
        UpdateCap(dt);

        // --- BRUSHES AND MOPS ---
        UpdateBrushes(dt);
    }

    void RotateWheelMesh(Transform wheel, float linearSpeed, float radius, float dt)
    {
        if (wheel == null) return;
        float rotDeg = (linearSpeed / radius) * Mathf.Rad2Deg * dt;
        wheel.Rotate(Vector3.right, rotDeg, Space.Self);
    }

    void UpdateCaster(Transform pivot, Transform wheel, float dt)
    {
        if (pivot == null || wheel == null) return;

        // Compute velocity at caster position from our kinematics
        Vector3 vel = GetCasterVelocityManual(pivot, linearVel, angularVel);
        Vector3 flatVel = new Vector3(vel.x, 0f, vel.z);

        // Swivel pivot toward local velocity direction
        if (flatVel.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatVel.normalized, Vector3.up);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, targetRot, 0.3f);
        }

        // Spin caster wheel
        float spinSpeed = flatVel.magnitude / casterWheelRadius;
        float rotDeg = spinSpeed * Mathf.Rad2Deg * dt;
        wheel.Rotate(Vector3.right, rotDeg, Space.Self);
    }

    Vector3 GetCasterVelocityManual(Transform pivot, float linearVel, float angularVel)
    {
        // Body forward motion
        Vector3 v = cachedTransform.forward * linearVel;

        // Rotational contribution: ω × r
        Vector3 r = pivot.position - cachedTransform.position;
        Vector3 angular = Vector3.up * angularVel;
        Vector3 rotVel = Vector3.Cross(angular, r);

        return v + rotVel;
    }

    void UpdateCap(float dt)
    {
        if (cap != null)
        {
            Vector3 targetPos = capStartingPosition;
            if (brushEnabled || mopsEnabled || rollersEnabled)
            {
                targetPos.y += capOffset;
            }
            float safeEasing = Mathf.Max(SAFE_MIN_EASING, capEasing);
            cap.localPosition = Vector3.MoveTowards(cap.localPosition, targetPos, (capOffset / safeEasing) * dt);
        }

        // target emission intensity
        float matTargetEmission = 2.8f;

        float desiredEmission = (brushEnabled || mopsEnabled || rollersEnabled) ? matTargetEmission : 0f;

        // avoid division by zero if capEasing is 0 in inspector
        float safeCapEasing = Mathf.Max(SAFE_MIN_EASING, capEasing);
        float step = (matTargetEmission / safeCapEasing) * dt;

        // move current emission toward desired value and clamp
        currentMatEmission = Mathf.MoveTowards(currentMatEmission, desiredEmission, step);
        currentMatEmission = Mathf.Clamp(currentMatEmission, 0f, matTargetEmission);

        if (material != null && material.HasProperty("_EmissionColor"))
        {
            // enable/disable emission keyword based on actual intensity
            if (currentMatEmission > 0f)
                material.EnableKeyword("_EMISSION");
            else
                material.DisableKeyword("_EMISSION");

            // use preserved base hue and scale by script-controlled intensity
            Color emissionColor = baseEmissionColor * currentMatEmission;
            material.SetColor("_EmissionColor", emissionColor);
        }
    }

    void UpdateBrushes(float dt)
    {
        // --- BRUSH ---
        targetBrushRPM = brushEnabled ? brushRPM : 0f;
        currentBrushRPM = Mathf.MoveTowards(currentBrushRPM, targetBrushRPM, (brushRPM / Mathf.Max(SAFE_MIN_EASING, brushSpinEasing)) * dt);
        float degreesPerSecond = currentBrushRPM * RPM_TO_DEG_PER_SEC; // 1 RPM = 6 deg/sec
        float rotDeg = degreesPerSecond * dt;

        if (brush != null)
        {
            brush.Rotate(Vector3.up, -rotDeg, Space.Self);
        }

        // --- MOPS ---
        targetMopRPM = mopsEnabled ? mopRPM : 0f;
        currentMopRPM = Mathf.MoveTowards(currentMopRPM, targetMopRPM, (mopRPM / Mathf.Max(SAFE_MIN_EASING, mopSpinEasing)) * dt);
        float mopDegreesPerSecond = currentMopRPM * RPM_TO_DEG_PER_SEC; // 1 RPM = 6 deg/sec
        float mopRotDeg = mopDegreesPerSecond * dt;

        if (mopR != null)
        {
            mopR.Rotate(Vector3.up, mopRotDeg, Space.Self);
        }

        if (mopL != null)
        {
            mopL.Rotate(Vector3.up, -mopRotDeg, Space.Self);
        }

        // --- ROLLERS ---
        targetRollerRPM = rollersEnabled ? rollerRPM : 0f;
        currentRollerRPM = Mathf.MoveTowards(currentRollerRPM, targetRollerRPM, (rollerRPM / Mathf.Max(SAFE_MIN_EASING, rollerSpinEasing)) * dt);
        float rollerDegreesPerSecond = currentRollerRPM * RPM_TO_DEG_PER_SEC; // 1 RPM = 6 deg/sec
        float rollerRotDeg = rollerDegreesPerSecond * dt;

        if (rollers != null)
        {
            foreach (var roller in rollers)
            {
                if (roller != null)
                    roller.Rotate(Vector3.right, rollerRotDeg, Space.Self);
            }
        }
    }
}