using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LineConnector : MonoBehaviour
{
    public RectTransform pointA;
    public RectTransform pointB;

    private RectTransform lineRect;
    private Image lineImage;


    [Header("Line Settings")] public float thickness = 2f;

    public void Initialize(DraggableVertex fromVertex, DraggableVertex toVertex)
    {
        pointA = fromVertex.rectTransform;
        pointB = toVertex.rectTransform;
        fromVertex.OnVertexDragged += UpdatePosition;
        toVertex.OnVertexDragged += UpdatePosition;
        UpdatePosition();

        var adder = GetComponentInChildren<VertexAdder>();
        if (adder != null) adder.Initialize(pointA, pointB);
    }

    private void Awake()
    {
        lineRect = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();

        lineRect.pivot = new Vector2(0.5f, 0.5f);
    }

    private void UpdatePosition()
    {
        if (pointA == null || pointB == null || this == null) return;

        var worldA = pointA.position;
        var worldB = pointB.position;

        lineRect.position = (worldA + worldB) / 2f;

        var dir = (worldB - worldA).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        var distance = Vector3.Distance(worldA, worldB);
        lineRect.sizeDelta = new Vector2(distance, thickness);
    }
}