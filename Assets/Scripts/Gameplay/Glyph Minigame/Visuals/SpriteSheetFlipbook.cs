using System.Collections;
using UnityEngine;

// Reproduce una secuencia de sprites (ex-GIF, ya recortado en frames) en
// loop durante una duración fija, y después se oculta sola.
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSheetFlipbook : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float framesPerSecond = 12f;

    private Coroutine playRoutine;

    // Reproduce los frames en loop durante totalDuration segundos y se oculta.
    public void Play(Sprite[] frames, float totalDuration)
    {
        if (frames == null || frames.Length == 0) return;

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlayRoutine(frames, totalDuration));
    }

    public void Stop()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = null;
        spriteRenderer.enabled = false;
    }

    private IEnumerator PlayRoutine(Sprite[] frames, float totalDuration)
    {
        spriteRenderer.enabled = true;

        float frameDuration = 1f / framesPerSecond;
        float elapsed = 0f;
        int frameIndex = 0;

        while (elapsed < totalDuration)
        {
            spriteRenderer.sprite = frames[frameIndex % frames.Length];
            frameIndex++;

            yield return new WaitForSeconds(frameDuration);
            elapsed += frameDuration;
        }

        spriteRenderer.enabled = false;
        playRoutine = null;
    }
}
