using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DraggableVertex : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform rectTransform { get; private set; }
    private Canvas canvas;
    private RectTransform visuals;
    public event Action OnVertexDragged;

    [SerializeField] private LineConnector lineConnectorPrefab;


    [Tooltip("Соседние вершины, к которым будут рисоваться линии в редакторе")]
    public DraggableVertex[] connectedVertices;

    public Dictionary<DraggableVertex, LineConnector> verticesDict;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        visuals = transform.GetChild(0).GetComponent<RectTransform>();

        verticesDict = connectedVertices
            .Where(v => v != null)
            .ToDictionary(
                v => v,
                v => (LineConnector)null
            );
    }

    private void Start()
    {
        foreach (var otherVertex in connectedVertices)
        {
            if (otherVertex == null) continue;

            if (otherVertex.verticesDict != null &&
                otherVertex.verticesDict.TryGetValue(this, out var existingLine))
            {
                verticesDict[otherVertex] = existingLine;
            }
            else
            {
                var line = Instantiate(lineConnectorPrefab, transform.parent);
                line.Initialize(this, otherVertex);
                line.transform.SetAsFirstSibling();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        visuals.localScale = Vector3.one * 1.2f;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        OnVertexDragged?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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
                .ToArray();
    }
#endif
}