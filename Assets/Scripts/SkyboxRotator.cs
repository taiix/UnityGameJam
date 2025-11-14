using UnityEngine;
[AddComponentMenu("Rendering/Skybox Rotator")]
[DisallowMultipleComponent]
public class SkyboxRotator : MonoBehaviour
{
        [Header("Target")]
        [Tooltip("Use the current RenderSettings.skybox. If false, use 'overrideMaterial'.")]
        [SerializeField] private bool useRenderSettingsSkybox = true;
        [Tooltip("Optional specific skybox material (must use Skybox/Cubemap or Skybox/Panoramic with a Rotation property).")]
        [SerializeField] private Material overrideMaterial;
        [Tooltip("Clone the material at runtime so the project asset isn't modified.")]
        [SerializeField] private bool cloneMaterial = true;

        [Header("Rotation")]
        [Tooltip("Rotation speed in degrees per second (Y-axis).")]
        [SerializeField] private float degreesPerSecond = 1f;
        [Tooltip("Start the skybox at a random angle on play.")]
        [SerializeField] private bool randomStartAngle = false;
        [Tooltip("Use unscaled time (ignores Time.timeScale).")]
        [SerializeField] private bool useUnscaledTime = true;

        [Header("Environment Updates")]
        [Tooltip("If enabled, periodically updates ambient/probes for the rotated skybox (costly).")]
        [SerializeField] private bool updateGlobalIllumination = false;
        [Tooltip("How often (seconds) to call DynamicGI.UpdateEnvironment().")]
        [SerializeField] private float giUpdateInterval = 0.5f;

        [Header("Advanced")]
        [Tooltip("Shader property name for rotation (degrees). For Unity skybox cubemap/panoramic this is usually _Rotation.")]
        [SerializeField] private string rotationProperty = "_Rotation";

        private Material runtimeMat;
        private int rotationId;
        private float angle;
        private float giTimer;
        private bool hasRotationProp;

        private void Awake()
        {
            rotationId = Shader.PropertyToID(rotationProperty);

            // Pick the material
            runtimeMat = useRenderSettingsSkybox ? RenderSettings.skybox : overrideMaterial;
            if (runtimeMat == null)
            {
                Debug.LogWarning("SkyboxCubemapRotator: No skybox material found/assigned.");
                enabled = false;
                return;
            }

            // Clone to avoid editing the asset
            if (cloneMaterial)
            {
                runtimeMat = new Material(runtimeMat);
                if (useRenderSettingsSkybox) RenderSettings.skybox = runtimeMat;
            }

            hasRotationProp = runtimeMat.HasProperty(rotationId);
            if (!hasRotationProp)
            {
                Debug.LogWarning($"SkyboxCubemapRotator: Material '{runtimeMat.name}' has no '{rotationProperty}' property. " +
                                 "Use Skybox/Cubemap or Skybox/Panoramic (they expose _Rotation).");
                enabled = false;
                return;
            }

            angle = randomStartAngle ? Random.Range(0f, 360f) : runtimeMat.GetFloat(rotationId);
            runtimeMat.SetFloat(rotationId, angle);
        }

        private void Update()
        {
            if (runtimeMat == null || !hasRotationProp) return;

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            angle = Mathf.Repeat(angle + degreesPerSecond * dt, 360f);
            runtimeMat.SetFloat(rotationId, angle);

            if (updateGlobalIllumination)
            {
                giTimer += dt;
                if (giTimer >= giUpdateInterval)
                {
                    giTimer = 0f;
                    DynamicGI.UpdateEnvironment(); // expensive; keep interval reasonable
                }
            }
        }
}
