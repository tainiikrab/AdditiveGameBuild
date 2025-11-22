using UnityEngine;
using UnityEngine.UI;

public class BrushPainter : MonoBehaviour
{
    public Camera paintCamera;
    public Texture2D brushTexture;
    [Range(0.005f, 0.5f)] public float brushSize = 0.05f;
    public Color brushColor = Color.red;
    public LayerMask paintLayerMask = ~0;
    public Material stampMaterial;

    public Slider brushSizeSlider;

    void Start()
    {
        if (brushSizeSlider != null)
        {
            brushSizeSlider.minValue = 0.005f;
            brushSizeSlider.maxValue = 0.5f;
            brushSizeSlider.value = brushSize;

            brushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
        }
    }

    void OnBrushSizeChanged(float value)
    {
        brushSize = value;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) {
            HandlePaint(Input.mousePosition);
        }
    }

    void HandlePaint(Vector3 screenPos)
    {
        if (paintCamera == null) paintCamera = Camera.main;
        Ray ray = paintCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, paintLayerMask))
        {
            var rend = hit.collider.GetComponent<Renderer>();
            if (rend == null) {
                Debug.LogWarning("Hit collider but no Renderer");
                return;
            }

            var paintable = hit.collider.GetComponent<PaintableObject>();
            if (paintable == null) {
                Debug.LogWarning("Hit collider but no PaintableObject component");
                return;
            }

            Vector2 uv = hit.textureCoord;

            if (paintable.paintTexture == null) {
                Debug.LogError("PaintableObject.paintTexture is null");
                return;
            }

            if (stampMaterial == null) {
                Debug.LogError("stampMaterial not assigned on BrushPainter");
                return;
            }

            if (!stampMaterial.shader.isSupported) {
                Debug.LogError("stampMaterial shader not supported on this platform/pipeline");
                return;
            }

            stampMaterial.SetTexture("_BrushTex", brushTexture);
            stampMaterial.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));
            stampMaterial.SetFloat("_BrushSize", brushSize);
            stampMaterial.SetColor("_Color", brushColor);

            RenderTexture tmp = RenderTexture.GetTemporary(
                paintable.paintTexture.descriptor.width,
                paintable.paintTexture.descriptor.height,
                0,
                paintable.paintTexture.descriptor.graphicsFormat);

            Graphics.Blit(paintable.paintTexture, tmp);

            stampMaterial.SetTexture("_MainTex", tmp);

            RenderTexture prev = RenderTexture.active;
            Graphics.Blit(tmp, paintable.paintTexture, stampMaterial);
            RenderTexture.active = prev;

            RenderTexture.ReleaseTemporary(tmp);
        }
    }
}
