using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public class GlassController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform glassRoot;
    [SerializeField] private Image glassImage;
    [SerializeField] private Sprite emptyGlassSprite;
    [SerializeField] private RectTransform sideAnchor;
    [SerializeField] private RectTransform centerAnchor;
    [SerializeField] private GameObject hintGO;

    [Header("Config")]
    [SerializeField] private float appearDuration = 0.4f;
    [SerializeField] private float disappearDuration = 0.2f;
    [SerializeField] private float popInDuration = 0.5f;
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CanvasGroup canvasGroup;
    private Coroutine currentRoutine;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    public void ShowEmptyGlass()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        glassImage.sprite = emptyGlassSprite;
        glassRoot.position = sideAnchor.position;
        glassRoot.localScale = Vector3.one * 0.85f;
        currentRoutine = StartCoroutine(FadeScaleRoutine(0f, 1f, 0.85f, 1f, appearDuration));
    }
    public void Hide()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(FadeScaleRoutine(canvasGroup.alpha, 0f, glassRoot.localScale.x, 0.7f, disappearDuration));
    }
    public void PlayWinSequence(Sprite fullSprite, System.Action onComplete)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(WinSequenceRoutine(fullSprite, onComplete));
    }
    private IEnumerator WinSequenceRoutine(Sprite fullSprite, System.Action onComplete)
    {
        yield return FadeScaleRoutine(canvasGroup.alpha, 0f, glassRoot.localScale.x, 0.6f, disappearDuration);

        glassImage.sprite = fullSprite;
        glassRoot.position = centerAnchor.position;
        glassRoot.localScale = Vector3.zero;
        canvasGroup.alpha = 1f;

        float t = 0f;
        while (t < popInDuration)
        {
            t += Time.deltaTime;
            float p = easeCurve.Evaluate(Mathf.Clamp01(t / popInDuration));
            glassRoot.localScale = Vector3.one * Mathf.Lerp(0f, popScale, p);
            yield return null;
        }
        glassRoot.localScale = Vector3.one * popScale;
        yield return PunchScale(Vector3.one * popScale, 1.1f, 0.15f);

        onComplete?.Invoke();
    }
    private IEnumerator FadeScaleRoutine(float fromAlpha, float toAlpha, float fromScale, float toScale, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = easeCurve.Evaluate(Mathf.Clamp01(t / duration));
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, p);
            glassRoot.localScale = Vector3.one * Mathf.Lerp(fromScale, toScale, p);
            yield return null;
        }
        canvasGroup.alpha = toAlpha;
        glassRoot.localScale = Vector3.one * toScale;
    }
    private IEnumerator PunchScale(Vector3 baseScale, float punchMultiplier, float duration)
    {
        float half = duration / 2f;
        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            glassRoot.localScale = Vector3.Lerp(baseScale, baseScale * punchMultiplier, t / half);
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            glassRoot.localScale = Vector3.Lerp(baseScale * punchMultiplier, baseScale, t / half);
            yield return null;
        }
        glassRoot.localScale = baseScale;
    }
}