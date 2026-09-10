using UnityEngine;

public class GlyphNodesRenderer : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private PatternRecognizer recognizer;
    [SerializeField] private SoulEnergy soul;
    [SerializeField] private SoulCreatureView creature;
    [SerializeField] private PatternNodeVisual nodeVisualPrefab;
    [SerializeField] private Transform nodesParent;

    private DrawPattern currentPattern;
    private PatternNodeVisual indicator;

    private void OnEnable()
    {
        caster.OnPatternChanged += HandlePatternChanged;
        creature.OnCreatureGone += HandleCreatureGone;
        recognizer.OnNodeHit += HandleNodeHit;
        recognizer.OnPatternComplete += HandlePatternComplete;
    }

    private void OnDisable()
    {
        caster.OnPatternChanged -= HandlePatternChanged;
        creature.OnCreatureGone -= HandleCreatureGone;
        recognizer.OnNodeHit -= HandleNodeHit;
        recognizer.OnPatternComplete -= HandlePatternComplete;
    }

    // Igual que GlyphReferenceDisplay: el punto de inicio recién aparece
    // cuando el glifo se revela de verdad, no antes.
    private void HandlePatternChanged(DrawPattern pattern)
    {
        currentPattern = pattern;

        if (soul.IsAvailable)
            ShowNode(0);
        else
            HideIndicator();
    }

    private void HandleCreatureGone() => ShowNode(0);

    // Solo marca el punto de inicio: apenas tocás el primer nodo, se oculta
    // y no vuelve a aparecer guiando los siguientes.
    private void HandleNodeHit(int index)
    {
        if (index == 0)
            HideIndicator();
    }

    private void HandlePatternComplete(float accuracy)
    {
        HideIndicator();
    }

    // Muestra el marcador en la posición del nodo indicado (usado solo para
    // el punto de inicio).
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
