using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DraggableVertex : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerClickHandler
{
    public RectTransform rectTransform { get; private set; }
    private Canvas canvas;
    private RectTransform visuals;

    public Vector2 postion => rectTransform.anchoredPosition;
    public event Action OnVertexDragged;

    [SerializeField] private LineConnector lineConnectorPrefab;

    [SerializeField] public bool isReference = false;

    [Tooltip("Соседние вершины, к которым будут рисоваться линии")] [SerializeField]
    private List<DraggableVertex> connectedVertices = new();

    public Dictionary<DraggableVertex, LineConnector> connectedVerticesDict { get; private set; }

    public void AddConnection(DraggableVertex otherVertex)
    {
        if (connectedVertices.Contains(otherVertex) || otherVertex == this) return;
        connectedVertices.Add(otherVertex);
        if (otherVertex.connectedVerticesDict.ContainsKey(this) && otherVertex.connectedVerticesDict[this] != null)
            connectedVerticesDict.Add(otherVertex, otherVertex.connectedVerticesDict[this]);
        else
            connectedVerticesDict.Add(otherVertex, null);
        InitializeConnections();
    }

    public void RemoveConnection(DraggableVertex otherVertex)
    {
        if (!connectedVertices.Remove(otherVertex)) return;
        if (connectedVerticesDict[otherVertex] != null)
            Destroy(connectedVerticesDict[otherVertex].gameObject);
        connectedVerticesDict.Remove(otherVertex);
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        visuals = transform.GetChild(0).GetComponent<RectTransform>();

        connectedVerticesDict = connectedVertices
            .Where(v => v != null)
            .ToDictionary(
                v => v,
                v => (LineConnector)null
            );
    }

    private void Start()
    {
        InitializeConnections();
    }

    public void InitializeConnections()
    {
        var connectedVerticesCopy = new List<DraggableVertex>(connectedVertices);

        foreach (var otherVertex in connectedVerticesCopy)
        {
            if (otherVertex == null || connectedVerticesDict[otherVertex] != null) continue;

            if (otherVertex.connectedVerticesDict != null &&
                otherVertex.connectedVerticesDict.TryGetValue(this, out var existingLine))
            {
                connectedVerticesDict[otherVertex] = existingLine;
            }
            else
            {
                var line = Instantiate(lineConnectorPrefab, transform.parent);
                line.Initialize(this, otherVertex);
                line.transform.SetAsFirstSibling();
                connectedVerticesDict[otherVertex] = line;
                otherVertex.AddConnection(this);
            }
        }
    }

    private float _lastClickTime;
    private const float DoubleClickThreshold = 0.3f;

    public void OnPointerClick(PointerEventData eventData)
    {
        var timeSinceLastClick = Time.time - _lastClickTime;
        _lastClickTime = Time.time;

        if (timeSinceLastClick <= DoubleClickThreshold) RemoveSelf();
    }

    public void RemoveSelf()
    {
        if (ModellingMinigameManager.playerVertices.Count <= 3) return;
        var neighbors = connectedVertices.ToList();

        foreach (var vertex in neighbors)
        {
            vertex.RemoveConnection(this);

            if (connectedVerticesDict.TryGetValue(vertex, out var line) && line != null) Destroy(line.gameObject);

            connectedVerticesDict.Remove(vertex);
        }

        for (var i = 0; i < neighbors.Count; i++)
        for (var j = i + 1; j < neighbors.Count; j++)
            neighbors[i].AddConnection(neighbors[j]);

        connectedVertices.Clear();

        ModellingMinigameManager.RemovePlayerVertex(this);
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isReference) return;
        visuals.localScale = Vector3.one * 1.2f;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isReference) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        OnVertexDragged?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isReference) return;
        visuals.localScale = Vector3.one;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (connectedVertices == null) return;

        Gizmos.color = Color.cyan;

        foreach (var v in connectedVertices)
        {
            if (v == null) continue;
            Gizmos.DrawLine(transform.position, v.transform.position);
        }
    }

    // убирает дубликаты
    private void OnValidate()
    {
        if (connectedVertices != null)
            connectedVertices = connectedVertices
                .Where(v => v != null)
                .Distinct()
                .ToList();
    }
#endif
}