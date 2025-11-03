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

    private static float CalculateDistance(IList<DraggableVertex> start, IList<DraggableVertex> end)
    {
        var avgMinVertexDist = 0f;
        var count = 1;
        foreach (var vertex in start)
        {
            var minDist = float.MaxValue;

            foreach (var referenceVertex in end)
            {
                minDist = Mathf.Min(minDist, Vector2.Distance(vertex.postion, referenceVertex.postion));
                if (minDist < 15)
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
}