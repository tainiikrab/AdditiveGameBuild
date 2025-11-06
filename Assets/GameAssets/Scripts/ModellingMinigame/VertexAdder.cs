using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VertexAdder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private DraggableVertex vertexPrefab;
    [SerializeField] private Image ghostVertexPrefab;
    [SerializeField] private Color ghostColor;
    [SerializeField] private float ghostScale;
    [SerializeField] private int maxVertices = 20;
    private RectTransform ghostVertex;
    private RectTransform pointA, pointB;
    // public Vector2 MidPoint => (pointA.anchoredPosition + pointB.anchoredPosition) / 2f;

    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Initialize(RectTransform pointA, RectTransform pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ModellingMinigameManager.playerVertices.Count >= maxVertices)
        {
            Debug.Log(ModellingMinigameManager.playerVertices.Count);
            return;
        }

        if (ghostVertex == null)
        {
            ghostVertex = Instantiate(ghostVertexPrefab, transform.parent).GetComponent<RectTransform>();
            ghostVertex.SetParent(transform.parent);
            ghostVertex.anchoredPosition = rt.anchoredPosition;
            var img = ghostVertex.GetComponent<Image>();
            img.raycastTarget = false;

            ghostVertex.localScale = Vector3.one * ghostScale;
            img.color = ghostColor;
        }
    }

    public void SplitEdge()
    {
        var vertexA = pointA.GetComponent<DraggableVertex>();
        var vertexB = pointB.GetComponent<DraggableVertex>();
        var newVertex = Instantiate(vertexPrefab, transform.parent);
        newVertex.GetComponent<RectTransform>().anchoredPosition = ghostVertex.anchoredPosition;
        newVertex.transform.SetParent(transform.parent.parent);
        ModellingMinigameManager.AddPlayerVertex(newVertex);
        vertexA.RemoveConnection(vertexB);
        vertexB.RemoveConnection(vertexA);
        vertexA.AddConnection(newVertex);
        vertexB.AddConnection(newVertex);

        newVertex.InitializeConnections();

        Destroy(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ghostVertex != null)
        {
            Destroy(ghostVertex.gameObject);
            ghostVertex = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ghostVertex != null)
        {
            SplitEdge();
            Destroy(ghostVertex.gameObject);
        }
    }
}