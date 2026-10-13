

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LoadingScreenScaler : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    // Referencias y datos de seguimiento.
    private SpriteRenderer background;
    private int previousScreenWidth;
    private int previousScreenHeight;
    private float previousCameraAspect;
    private float previousOrthographicSize;

    /*
     * Inicializa el fondo y obtiene la cámara.
     * Realiza el primer ajuste para cubrir la pantalla.
     */
    private void Awake()
    {
        // Variables locales.
        Camera mainCamera = Camera.main;

        // Inicialización.
        background = GetComponent<SpriteRenderer>();

        if (targetCamera == null)
        {
            targetCamera = mainCamera;
        }

        UpdateBackgroundScale();
    }

    /*
     * Comprueba si cambió la resolución o el tamaño visible
     * de la cámara. Solo recalcula la escala cuando es necesario.
     */
    private void LateUpdate()
    {
        // Variables locales.
        bool resolutionChanged;
        bool cameraChanged;

        if (targetCamera == null || background == null)
        {
            return;
        }

        resolutionChanged =
            Screen.width != previousScreenWidth ||
            Screen.height != previousScreenHeight;

        cameraChanged =
            !Mathf.Approximately(
                targetCamera.aspect,
                previousCameraAspect
            ) ||
            !Mathf.Approximately(
                targetCamera.orthographicSize,
                previousOrthographicSize
            );

        if (resolutionChanged || cameraChanged)
        {
            UpdateBackgroundScale();
        }
    }

    /*
     * Ajusta el fondo para cubrir el área visible de la cámara.
     * Calcula X e Y de forma independiente para permitir que
     * la imagen se deforme si cambia la relación de aspecto.
     * No modifica la posición ni la escala local del cartel hijo.
     */
    private void UpdateBackgroundScale()
    {
        // Variables locales.
        float visibleHeight;
        float visibleWidth;
        float spriteWidth;
        float spriteHeight;
        float parentScaleX;
        float parentScaleY;
        float requiredScaleX;
        float requiredScaleY;
        float overscan = 1.02f;
        Vector3 parentLossyScale;

        if (targetCamera == null ||
            background == null ||
            background.sprite == null ||
            !targetCamera.orthographic)
        {
            return;
        }

        // Calcula el área visible en unidades del mundo.
        visibleHeight = targetCamera.orthographicSize * 2f;
        visibleWidth = visibleHeight * targetCamera.aspect;

        // Obtiene el tamaño original del sprite.
        spriteWidth = background.sprite.bounds.size.x;
        spriteHeight = background.sprite.bounds.size.y;

        if (spriteWidth <= 0f || spriteHeight <= 0f)
        {
            return;
        }

        // Considera la escala del objeto padre, si existe.
        parentLossyScale = transform.parent != null
            ? transform.parent.lossyScale
            : Vector3.one;

        parentScaleX = Mathf.Abs(parentLossyScale.x);
        parentScaleY = Mathf.Abs(parentLossyScale.y);

        if (parentScaleX <= 0f || parentScaleY <= 0f)
        {
            return;
        }

        // Calcula la escala necesaria para cada eje.
        requiredScaleX =
            (visibleWidth / (spriteWidth * parentScaleX)) * overscan;

        requiredScaleY =
            (visibleHeight / (spriteHeight * parentScaleY)) * overscan;

        // Aplica el ajuste sin alterar la posición del fondo.
        transform.localScale = new Vector3(
            requiredScaleX,
            requiredScaleY,
            transform.localScale.z
        );

        // Guarda los valores actuales para la siguiente comprobación.
        previousScreenWidth = Screen.width;
        previousScreenHeight = Screen.height;
        previousCameraAspect = targetCamera.aspect;
        previousOrthographicSize = targetCamera.orthographicSize;
    }
}
