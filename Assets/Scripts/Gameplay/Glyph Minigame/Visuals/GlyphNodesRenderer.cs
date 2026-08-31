using UnityEngine;

public class GlyphNodesRenderer : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private PatternRecognizer recognizer;
    [SerializeField] private PatternNodeVisual nodeVisualPrefab;
    [SerializeField] private Transform nodesParent;

    private DrawPattern currentPattern;
    private PatternNodeVisual indicator;

    private void OnEnable()
    {
        caster.OnPatternChanged += HandlePatternChanged;
        recognizer.OnNodeHit += HandleNodeHit;
        recognizer.OnPatternComplete += HandlePatternComplete;
    }

    private void OnDisable()
    {
        caster.OnPatternChanged -= HandlePatternChanged;
        recognizer.OnNodeHit -= HandleNodeHit;
        recognizer.OnPatternComplete -= HandlePatternComplete;
    }

    private void HandlePatternChanged(DrawPattern pattern)
    {
        currentPattern = pattern;
        ShowNode(0);
    }

    private void HandleNodeHit(int index)
    {
        ShowNode(index + 1);
    }

    private void HandlePatternComplete(float accuracy)
    {
        HideIndicator();
    }

    // Muestra el marcador en la posición del próximo nodo esperado.
    private void ShowNode(int index)
    {
        if (currentPattern == null || nodeVisualPrefab == null || index < 0 || index >= currentPattern.nodes.Count)
        {
            HideIndicator();
            return;
        }

        Vector3 position = currentPattern.nodes[index].position;

        if (indicator == null)
            indicator = Instantiate(nodeVisualPrefab, position, Quaternion.identity, nodesParent);
        else
        {
            indicator.gameObject.SetActive(true);
            indicator.transform.position = position;
        }

        indicator.ResetVisual();
    }

    private void HideIndicator()
    {
        if (indicator != null)
            indicator.gameObject.SetActive(false);
    }
}
