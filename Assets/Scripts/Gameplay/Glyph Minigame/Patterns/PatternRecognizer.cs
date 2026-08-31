using System;
using UnityEngine;

public class PatternRecognizer : MonoBehaviour
{
    [SerializeField] private float maxTimeBetweenNodes = 1.5f;

    private DrawPattern pattern;
    private int currentNodeIndex;
    private int nodesHitCorrectly;
    private float lastNodeTime;
    private bool isDrawing;

    public event Action<int> OnNodeHit;
    public event Action<float> OnPatternComplete;

    public void SetPattern(DrawPattern newPattern)
    {
        pattern = newPattern;
    }

    public void StartDrawing()
    {
        if (pattern == null || pattern.nodes.Count == 0) return;

        isDrawing = true;
        currentNodeIndex = 0;
        nodesHitCorrectly = 0;
        lastNodeTime = Time.time;
    }

    public void UpdateDrawing(Vector2 worldPos)
    {
        if (!isDrawing || currentNodeIndex >= pattern.nodes.Count) return;

        if (Time.time - lastNodeTime > maxTimeBetweenNodes)
        {
            Finish();
            return;
        }

        PatternNode target = pattern.nodes[currentNodeIndex];
        if (Vector2.Distance(worldPos, target.position) <= target.radius)
        {
            nodesHitCorrectly++;
            OnNodeHit?.Invoke(currentNodeIndex);
            currentNodeIndex++;
            lastNodeTime = Time.time;

            if (currentNodeIndex >= pattern.nodes.Count)
                Finish();
        }
    }

    public void StopDrawing()
    {
        if (isDrawing) Finish();
    }

    // Calcula la precisión final y dispara OnPatternComplete.
    private void Finish()
    {
        isDrawing = false;
        float accuracy = pattern.nodes.Count > 0
            ? (float)nodesHitCorrectly / pattern.nodes.Count
            : 0f;
        OnPatternComplete?.Invoke(accuracy);
    }
}
