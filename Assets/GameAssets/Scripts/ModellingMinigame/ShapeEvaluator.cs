using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShapeEvaluator
{
    public static float Evaluate(
        IList<DraggableVertex> playerVertices,
        IList<DraggableVertex> referenceVertices,
        RectTransform commonRoot,
        float maxDist = 300f,
        float weightRefToPlayer = 0.7f,
        float weightPlayerToRef = 0.3f,
        bool loopReference = true
    )
    {
        var avgPtR = CalculateDistance(playerVertices, referenceVertices);
        var avgRtP = CalculateDistance(referenceVertices, playerVertices);
        Debug.Log($"avgPtR: {avgPtR}, avgRtP: {avgRtP}");
        return avgPtR * weightPlayerToRef + avgRtP * weightRefToPlayer;
    }

    public static float CalculateDistance(IList<DraggableVertex> start, IList<DraggableVertex> end)
    {
        var avgMinVertexDist = 0f;
        var count = 1;

        foreach (var vertex in start)
        {
            var minDist = float.MaxValue;

            for (var i = 0; i < end.Count; i++)
            {
                var A = end[i].postion;
                var B = end[(i + 1) % end.Count].postion;
                var C = vertex.postion;

                var dist = DistancePointToSegment(A, B, C);
                minDist = Mathf.Min(minDist, dist);

                if (minDist < 15f)
                {
                    minDist = 0f;
                    break;
                }
            }

            avgMinVertexDist += (minDist - avgMinVertexDist) / count;
            count++;
        }

        return avgMinVertexDist;
    }

    private static float DistancePointToSegment(Vector2 A, Vector2 B, Vector2 C)
    {
        var AB = B - A;
        var AC = C - A;

        var ab2 = AB.sqrMagnitude;
        if (ab2 == 0f)
            return Vector2.Distance(A, C);

        var t = Vector2.Dot(AC, AB) / ab2;

        if (t < 0f) return Vector2.Distance(C, A);
        if (t > 1f) return Vector2.Distance(C, B);

        var P = A + t * AB;
        return Vector2.Distance(C, P);
    }
}