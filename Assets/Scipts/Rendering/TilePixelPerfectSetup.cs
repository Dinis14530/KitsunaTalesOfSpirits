using UnityEngine;

public class TilePixelPerfectSetup : MonoBehaviour
{
    [SerializeField] private bool forcePointFilterOnStart = true;
    [SerializeField] private bool disableTrilinearFiltering = true;

    private void Awake()
    {
        if (disableTrilinearFiltering)
            QualitySettings.globalTextureMipmapLimit = 0;

        if (forcePointFilterOnStart)
            SetAllSpritesPointFilter();
    }

    private void SetAllSpritesPointFilter()
    {
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sr in allSprites)
        {
            if (sr.sprite != null && sr.sprite.texture != null)
                sr.sprite.texture.filterMode = FilterMode.Point;
        }

        Texture2D[] allTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
        foreach (Texture2D tex in allTextures)
        {
            if (tex != null && (tex.name.Contains("Tile") || tex.name.Contains("tile")))
                tex.filterMode = FilterMode.Point;
        }
    }
}
