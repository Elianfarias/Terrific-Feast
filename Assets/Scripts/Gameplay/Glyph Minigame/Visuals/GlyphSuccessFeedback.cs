using UnityEngine;

// Sonido + camera shake cuando el glifo sale exitoso (sin importar el sabor).
public class GlyphSuccessFeedback : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private AudioClip successSound;

    private void OnEnable() => caster.OnInvocationResolved += HandleInvocationResolved;
    private void OnDisable() => caster.OnInvocationResolved -= HandleInvocationResolved;

    private void HandleInvocationResolved(GameObject result, DrinkRecipe usedRecipe, float accuracy)
    {
        bool success = usedRecipe != null && accuracy >= usedRecipe.RequiredAccuracy;
        if (!success) return;

        if (successSound != null && AudioController.Instance != null)
            AudioController.Instance.PlaySoundEffect(successSound);

        if (cameraShake != null)
            cameraShake.Shake();
    }
}
