using UnityEngine;

public class ModelRotator : MonoBehaviour
{
    public float rotationSpeed = 0.2f;
    public float inertiaDamping = 5f;

    private Vector2 angularVelocity;
    private bool isDragging;
    private Vector3 lastMousePos;
    public Transform model { get; private set; }


    private void Awake()
    {
        SetupModel();
    }

    private void SetupModel()
    {
        // if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);

        if (OrderManager.currentOrder == null)
        {
            Debug.LogWarning($"No current order. Fallback to {GlobalConfig.Instance.Orders[0].orderName}");
            OrderManager.SetCurrentOrder(GlobalConfig.Instance.Orders[0]);
        }

        var modelGO = Instantiate(OrderManager.currentOrder.mesh, transform);
        model = modelGO.transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.transform == transform)
            {
                isDragging = true;
                lastMousePos = Input.mousePosition;
                angularVelocity = Vector2.zero;
            }
        }

        if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (isDragging)
        {
            var mouseDelta = (Vector2)(Input.mousePosition - lastMousePos);
            var rotX = mouseDelta.y * rotationSpeed;
            var rotY = -mouseDelta.x * rotationSpeed;

            model.Rotate(Camera.main.transform.up, rotY, Space.World);
            model.Rotate(Camera.main.transform.right, rotX, Space.World);

            angularVelocity = new Vector2(rotX, rotY) / Time.deltaTime;
            lastMousePos = Input.mousePosition;
        }
        else
        {
            if (angularVelocity.sqrMagnitude > 0.01f)
            {
                var rotX = angularVelocity.x * Time.deltaTime;
                var rotY = angularVelocity.y * Time.deltaTime;

                model.Rotate(Camera.main.transform.up, rotY, Space.World);
                model.Rotate(Camera.main.transform.right, rotX, Space.World);

                angularVelocity = Vector2.Lerp(angularVelocity, Vector2.zero, inertiaDamping * Time.deltaTime);
            }
        }
    }
}