using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles mouse interaction with objects implementing IRaycastInteractable.
/// </summary>
public class MouseRaycaster : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;

    [SerializeField] private Vector2 defaultCursorHotspot;
    [SerializeField] private Vector2 hoverCursorHotspot;
    private Camera cam;
    private GameObject lastHovered;
    private IRaycastInteractable lastInteractable;

    private void Start()
    {
        cam = Camera.main;
        if (EventSystem.current == null) Debug.LogWarning("No EventSystem found in scene. UI blocking will not work.");
        Cursor.SetCursor(defaultCursor, defaultCursorHotspot, CursorMode.Auto);
    }

    private void Update()
    {
        HandleMouseInteraction();
    }

    private void HandleMouseInteraction()
    {
        if (IsPointerOverUI())
        {
            ExitLastInteractable();
            return;
        }

        var ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            ProcessHitObject(hit.collider.gameObject);
            HandleClick();
        }
        else
        {
            ExitLastInteractable();
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void ProcessHitObject(GameObject current)
    {
        if (current == lastHovered) return;

        ExitLastInteractable();

        if (current.TryGetComponent(out IRaycastInteractable interactable))
        {
            Cursor.SetCursor(hoverCursor, hoverCursorHotspot, CursorMode.Auto);
            lastHovered = current;
            lastInteractable = interactable;
            interactable.OnHoverEnter();
        }
        else
        {
            Cursor.SetCursor(defaultCursor, defaultCursorHotspot, CursorMode.Auto);
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
        if (IsPointerOverUI()) return;

        if (Input.GetMouseButtonDown(0) && lastInteractable != null)
            lastInteractable.OnClick();
    }

    private void LateUpdate()
    {
        if (lastInteractable != null)
            lastInteractable.OnHover();
    }
}