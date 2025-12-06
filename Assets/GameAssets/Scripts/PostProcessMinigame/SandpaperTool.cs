using UnityEngine;

public class SandpaperTool : AbstractTool
{
    [Header("Sandpaper Settings")] [SerializeField]
    private LayerMask modelLayer; // слой модели

    [SerializeField] private float rayDistance = 1f; // дальность рейкаста
    [SerializeField] private float smoothSpeed = 0.2f; // скорость "шлифовки"
    [SerializeField] private string smoothnessProperty = "_Smoothness"; // имя параметра

    private Material targetMaterial;
    private float defaultSmoothness = 0;
    private Renderer cachedRenderer;

    [SerializeField] private Animation animation;
    [SerializeField] private string animationName = "Sandpaper";

    protected override void OnActiveInstrument()
    {
        // Debug.Log("OnUse()");
        if (Physics.Raycast(transform.position, transform.forward, out var hit, rayDistance, modelLayer))
        {
            // Debug.Log("Raycast");
            var parent = hit.transform.parent;
            var renderer = parent.GetChild(parent.childCount - 1).GetComponentInChildren<Renderer>();
            Debug.Log(renderer.gameObject.name);
            if (renderer != null)
                // if (targetMaterial == null)
                //     // клонируем материал, чтобы не портить sharedMaterial
                //     targetMaterial = renderer.material;
                // Debug.Log("Renderer");

                if (renderer.material.HasProperty(smoothnessProperty))
                {
                    if (!isOnUse) return;

                    // Debug.Log("Smoothness");
                    var current = renderer.material.GetFloat(smoothnessProperty);
                    if (defaultSmoothness == 0)
                    {
                        defaultSmoothness = current;
                        cachedRenderer = renderer;
                    }

                    var newValue = Mathf.Clamp01(current + smoothSpeed * Time.deltaTime);
                    renderer.material.SetFloat(smoothnessProperty, newValue);
                    renderer.material.SetFloat("_Metallic", newValue / 2);
                    // звук шлифовки
                    audioSource.volume = 1f;
                    animation[animationName].speed = 1f;
                    Debug.Log($"Material smoothness: {renderer.material.GetFloat(smoothnessProperty)}");
                }
        }
        else
        {
            // Debug.Log("Else");
            audioSource.volume = 0f;
            animation[animationName].speed = 0f;
        }
    }

    private bool isOnUse = false;

    protected override void OnUse()
    {
        isOnUse = true;
    }

    protected override void OnStopUse()
    {
        isOnUse = false;
        OnStopActiveInstrument();
    }

    protected override void OnStopActiveInstrument()
    {
        audioSource.volume = 0f;
        animation[animationName].speed = 0f;
    }

    private void OnDestroy()
    {
        if (cachedRenderer == null) return;

        cachedRenderer?.material.SetFloat(smoothnessProperty, defaultSmoothness);
        cachedRenderer?.material.SetFloat("_Metallic", 0);
    }
}