using UnityEngine;
using UnityEngine.EventSystems;

public class ModelTurner : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private const string modelParentTag = "SlicedPrefabParent";
    private static Transform modelParent;
    public static GameObject turningModel;

    [SerializeField] private string layerName = "RenderTexture";
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float idleRotationSpeed = 2f;
    private bool isDragging;
    private bool isTargetFound;

    private void Awake()
    {
        if (turningModel == null)
            FindTurningModel();
    }

    private void Update()
    {
        if (!isTargetFound || isDragging) return;
        // Debug.Log(turningModel);
        turningModel.transform.Rotate(Vector3.up, idleRotationSpeed * Time.deltaTime, Space.World);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isTargetFound) return;
        var rotationY = -eventData.delta.x * rotationSpeed;
        turningModel.transform.Rotate(Vector3.up, rotationY, Space.World);
        // Debug.Log(rotationY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void FindTurningModel()
    {
        if (modelParent == null)
            modelParent = GameObject.FindGameObjectWithTag(modelParentTag).transform;

        if (modelParent != null && modelParent.childCount > 0)
        {
            turningModel = modelParent.GetChild(0).gameObject;
            isTargetFound = true;
        }
    }

    public void SetModel(GameObject model)
    {
        if (turningModel == null)
            FindTurningModel();

        if (turningModel != null)
            Destroy(turningModel.gameObject);

        turningModel = Instantiate(model, modelParent);
        SetLayerRecursively(turningModel.gameObject, LayerMask.NameToLayer(layerName));
        isTargetFound = true;
    }

    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}