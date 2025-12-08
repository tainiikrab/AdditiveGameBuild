using UnityEngine;

public class SandpaperTool : AbstractTool
{
    [Header("Sandpaper Settings")] [SerializeField]
    private LayerMask modelLayer;

    [SerializeField] private float rayDistance = 1f;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private string smoothnessProperty = "_Smoothness";

    [Header("Mouse Control")] [SerializeField]
    private float mouseSensitivity = 1.0f; // >1 = быстрее достигается макс скорость, <1 = сложнее

    private Material targetMaterial;
    private float defaultSmoothness = 0;
    private Renderer cachedRenderer;

    [SerializeField] private Animation animation;
    [SerializeField] private string animationName = "Sandpaper";

    private bool isOnUse = true;
    public static bool isSmoothing { get; private set; } = false;

    public static float smoothnessDone { get; private set; } = 0f;
    public static float requiredSmoothness { get; private set; } = 0.5f;

    private void Awake()
    {
        requiredSmoothness = Random.Range(0.1f, 1f);
    }

    private bool isFirstCall = true;

    protected override void OnActiveInstrument()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, rayDistance, modelLayer))
        {
            var parent = hit.transform.parent;
            var renderer = parent.GetChild(parent.childCount - 1).GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.material.HasProperty(smoothnessProperty))
            {
                if (!isOnUse) return;
                isSmoothing = true;
                float current = 0;
                if (!isFirstCall)
                    current = renderer.material.GetFloat(smoothnessProperty);
                else
                    isFirstCall = false;
                if (defaultSmoothness == 0)
                {
                    defaultSmoothness = current;
                    cachedRenderer = renderer;
                }


                var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude;

                var adjustedDelta = mouseDelta * mouseSensitivity;

                var effectiveSpeed = Mathf.Min(adjustedDelta, smoothSpeed);


                var newValue = Mathf.Clamp01(current + effectiveSpeed * Time.deltaTime);
                smoothnessDone = newValue;
                renderer.material.SetFloat(smoothnessProperty, newValue);
                renderer.material.SetFloat("_Metallic", newValue / 2);


                audioSource.volume = 1f;
                animation[animationName].speed = effectiveSpeed / smoothSpeed;

                // Debug.Log(
                //     $"Smoothness: {renderer.material.GetFloat(smoothnessProperty)} | Mouse speed: {mouseDelta} | Effective: {effectiveSpeed}");
            }
        }
        else
        {
            isSmoothing = false;
            audioSource.volume = 0f;
            animation[animationName].speed = 0f;
        }
    }

    protected override void OnUse()
    {
        ;
    }

    // protected override void OnStopUse()
    // {
    //     isOnUse = false;
    //     OnStopActiveInstrument();
    // }

    protected override void OnStopActiveInstrument()
    {
        audioSource.volume = 0f;
        isSmoothing = false;
        animation[animationName].speed = 0f;
    }

    private void OnDestroy()
    {
        if (cachedRenderer == null) return;

        cachedRenderer?.material.SetFloat(smoothnessProperty, defaultSmoothness);
        cachedRenderer?.material.SetFloat("_Metallic", 0);
    }
}