using UnityEngine;
using UnityEngine.SceneManagement;

// Al servir el trago, vuelve a la novela visual. YarnComands ya se encarga
// de retomar en el nodo correcto (resumeNode, guardado antes de venir acá).
public class ReturnToVisualNovel : MonoBehaviour
{
    [SerializeField] private FlavorDrawSequencer sequencer;

    private void OnEnable() => sequencer.OnTragoServed += HandleTragoServed;
    private void OnDisable() => sequencer.OnTragoServed -= HandleTragoServed;

    private void HandleTragoServed()
    {
        SceneManager.LoadScene("VisualNovelScene");
    }
}
