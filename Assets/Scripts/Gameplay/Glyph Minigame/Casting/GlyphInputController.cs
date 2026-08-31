using UnityEngine;

public class GlyphInputController : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private Camera worldCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            caster.OnEnterPressed();

        Vector2 worldPos = worldCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
            caster.OnDrawStart(worldPos);

        if (Input.GetMouseButton(0))
            caster.OnDrawUpdate(worldPos);

        if (Input.GetMouseButtonUp(0))
            caster.OnDrawEnd();
    }
}
