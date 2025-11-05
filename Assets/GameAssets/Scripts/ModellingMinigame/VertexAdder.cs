using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VertexAdder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private DraggableVertex vertexPrefab;
    [SerializeField] private Image ghostVertexPrefab;
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
        if (ghostVertex == null)
        {
            ghostVertex = Instantiate(ghostVertexPrefab, transform.parent).GetComponent<RectTransform>();
            ghostVertex.SetParent(transform.parent);
            var img = ghostVertex.GetComponent<Image>();
            img.raycastTarget = false;
            // var rt = ghostVertex.GetComponent<RectTransform>();
            // rt.sizeDelta = new Vector2(20, 20);
            // ghostVertex.anchoredPosition = MidPoint;

            // var img = ghostVertex.GetComponent<Image>();
            // img.color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void SplitEdge()
    {
        var vertexA = pointA.GetComponent<DraggableVertex>();
        var vertexB = pointB.GetComponent<DraggableVertex>();
        var newVertex = Instantiate(vertexPrefab, transform.parent.parent);
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