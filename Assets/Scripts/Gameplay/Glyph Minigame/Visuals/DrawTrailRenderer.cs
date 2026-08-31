using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawTrailRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private float minDistanceBetweenPoints = 0.05f;

    private readonly List<Vector3> points = new List<Vector3>();

    public void BeginTrail(Vector3 startPos)
    {
        points.Clear();
        points.Add(startPos);
        line.positionCount = 1;
        line.SetPosition(0, startPos);
    }

    public void AddPoint(Vector3 pos)
    {
        if (points.Count > 0 && Vector3.Distance(points[^1], pos) < minDistanceBetweenPoints)
            return;

        points.Add(pos);
        line.positionCount = points.Count;
        line.SetPosition(points.Count - 1, pos);
    }

    public void ClearTrail()
    {
        points.Clear();
        line.positionCount = 0;
    }
}
