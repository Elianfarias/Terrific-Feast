using System.Collections;
using UnityEngine;

public class PatternNodeVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color pendingColor = Color.gray;
    [SerializeField] private Color hitColor = Color.cyan;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private float failFlashDuration = 0.2f;

    public void ResetVisual()
    {
        StopAllCoroutines();
        sprite.color = pendingColor;
    }

    public void SetHit(bool hit)
    {
        sprite.color = hit ? hitColor : pendingColor;
    }

    public void PulseFail()
    {
        StopAllCoroutines();
        StartCoroutine(FlashFail());
    }

    private IEnumerator FlashFail()
    {
        sprite.color = failColor;
        yield return new WaitForSeconds(failFlashDuration);
        sprite.color = pendingColor;
    }
}
