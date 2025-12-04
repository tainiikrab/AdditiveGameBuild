using UnityEngine;
using UnityEngine.UI;

public class WallpaperChanger : MonoBehaviour
{
    [SerializeField] private Texture[] wallpapers;
    private int currentIndex;
    private RawImage spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<RawImage>();
        spriteRenderer.texture = wallpapers[currentIndex];
    }

    public void SetNextWallpaper()
    {
        currentIndex++;
        spriteRenderer.texture = wallpapers[currentIndex %= wallpapers.Length];
    }
}