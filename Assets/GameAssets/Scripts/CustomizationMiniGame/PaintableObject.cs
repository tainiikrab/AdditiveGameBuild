using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
public class PaintableObject : MonoBehaviour
{
    public int textureSize = 1024;
    public RenderTexture paintTexture;
    public Texture2D initialTexture;
    [Tooltip("Optional RawImage to debug paint texture")]
    public RawImage debugRawImage;

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        rend.material = new Material(rend.material);

        if (paintTexture == null) {
            RenderTextureDescriptor desc = new RenderTextureDescriptor(textureSize, textureSize,
                UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB, 0);
            paintTexture = new RenderTexture(desc);
            paintTexture.wrapMode = TextureWrapMode.Repeat;
            paintTexture.Create();

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = paintTexture;
            GL.Clear(true, true, new Color(0,0,0,0));
            RenderTexture.active = prev;
        }

        if (initialTexture != null) {
            RenderTexture prev = RenderTexture.active;
            Graphics.Blit(initialTexture, paintTexture);
            RenderTexture.active = prev;
        }

        if (rend.material.HasProperty("_PaintTex")) {
            rend.material.SetTexture("_PaintTex", paintTexture);
            Debug.Log($"{name}: _PaintTex assigned ({paintTexture.width}x{paintTexture.height})");
        } else {
            Debug.LogError($"{name}: Material does NOT have _PaintTex property. Check shader property reference.");
        }

        if (debugRawImage != null) {
            debugRawImage.texture = paintTexture;
        }
    }

    void OnDestroy()
    {
        if (paintTexture != null) {
            if (paintTexture.IsCreated()) paintTexture.Release();
            Destroy(paintTexture);
        }
    }
}
