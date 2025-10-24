using UnityEngine;

/// <summary>
/// Handles mouse interaction with objects implementing IRaycastInteractable.
/// Tracks hover states and triggers OnClick when clicking.
/// </summary>
public class MouseRaycaster : MonoBehaviour
{
    // [SerializeField] private LayerMask interactionLayer = Physics.DefaultRaycastLayers;
    // [SerializeField] private float maxInteractionDistance = 10000f;

    private Camera cam;
    private GameObject lastHovered;
    private IRaycastInteractable lastInteractable;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleMouseInteraction();
    }

    private void HandleMouseInteraction()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        var hasHit = Physics.Raycast(ray, out hit);

        if (hasHit)
        {
            ProcessHitObject(hit.collider.gameObject);
            HandleClick();
        }
        else
        {
            ExitLastInteractable();
        }
    }

    private void ProcessHitObject(GameObject current)
    {
        if (current == lastHovered) return;


        ExitLastInteractable();

        if (current.TryGetComponent(out IRaycastInteractable interactable))
        {
            lastHovered = current;
            lastInteractable = interactable;
            interactable.OnHoverEnter();
        }
    }

    private void ExitLastInteractable()
    {
        if (lastInteractable != null)
        {
            lastInteractable.OnHoverExit();
            lastInteractable = null;
            lastHovered = null;
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0) && lastInteractable != null) lastInteractable.OnClick();
    }

    private void LateUpdate()
    {
        // Optional: Call OnHover only after all physics updates
        if (lastInteractable != null) lastInteractable.OnHover();
    }
}