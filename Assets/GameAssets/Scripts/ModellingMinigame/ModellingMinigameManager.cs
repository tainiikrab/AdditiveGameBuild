using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModellingMinigameManager : MonoBehaviour
{
    [SerializeField] private RectTransform playerVerticesHolder;
    [SerializeField] private RectTransform referenceVerticesHolder;
    [SerializeField] private float maxDistPixels = 300f;
    [SerializeField] private bool isLoop = true;
    private static DraggableVertex[] referenceVertices;
    private static List<DraggableVertex> playerVertices;
    [SerializeField] private TextMeshProUGUI scoreText;

    private float score;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        referenceVertices = new DraggableVertex[referenceVerticesHolder.childCount];
        for (var i = 0; i < referenceVertices.Length; i++)
            referenceVertices[i] = referenceVerticesHolder.GetChild(i).GetComponent<DraggableVertex>();

        playerVertices = new List<DraggableVertex>();
        for (var i = 0; i < playerVerticesHolder.childCount; i++)
            playerVertices.Add(playerVerticesHolder.GetChild(i).GetComponent<DraggableVertex>());

        // foreach (var vertex in playerVertices) Debug.Log(vertex);
        // foreach (var vertex in referenceVertices) Debug.Log(vertex);
    }

    public static void AddPlayerVertex(DraggableVertex vertex)
    {
        playerVertices.Add(vertex);
    }

    public static void RemovePlayerVertex(DraggableVertex vertex)
    {
        playerVertices.Remove(vertex);
    }

    private void Update()
    {
        score = ShapeEvaluator.Evaluate(playerVertices, referenceVertices, rectTransform, maxDistPixels);

        scoreText.text = $"Error: {score}";
    }
}