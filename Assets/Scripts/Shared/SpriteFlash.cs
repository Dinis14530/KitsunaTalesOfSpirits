using System.Collections;
using UnityEngine;

public static class SpriteFlash
{
    public static IEnumerator Flash(SpriteRenderer renderer, Color hitColor, float duration)
    {
        if (renderer == null)
            yield break;

        Color originalColor = renderer.color;
        renderer.color = Color.Lerp(originalColor, hitColor, 0.7f);
        yield return new WaitForSeconds(duration);
        renderer.color = originalColor;
    }
}
