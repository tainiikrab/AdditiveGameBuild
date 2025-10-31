using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rellac.Windows
{
	/// <summary>
	///     Script to handle moving windows
	/// </summary>
	public class GUIWindowMover : GUIPointerObject, IDragHandler, IEndDragHandler
    {
	    /// <summary>
	    ///     Window to move
	    /// </summary>
	    [Tooltip("Window to move")] [SerializeField]
        private RectTransform parentWindow;

	    /// <summary>
	    ///     Mover is locked and unusable
	    /// </summary>
	    [Tooltip("Mover is locked and unusable")] [SerializeField]
        private bool isLocked;

	    /// <summary>
	    ///     Fires when a window has been moved
	    /// </summary>
	    [Tooltip("Fires when a window has been moved")] [SerializeField]
        private UnityEvent onWindowMoved;

        private Canvas canvas;

        private GUIWindowExpander expander;
        // private bool isGrabbed;

        private Vector2 mouseOffset;

        private void Awake()
        {
            expander = GetComponent<GUIWindowExpander>();
        }

        private void Start()
        {
            onPointerDown.AddListener(SetIsGrabbed);
            canvas = LaptopUIWindow.GetCanvas(transform.parent);
        }

        // private void Update()
        // {
        //     if (!isGrabbed || isLocked) return;
        //
        //     parentWindow.position = (Vector2)Input.mousePosition + mouseOffset;
        //     if (Input.GetMouseButtonUp(0))
        //     {
        //         isGrabbed = false;
        //         if (onWindowMoved != null) onWindowMoved.Invoke();
        //     }
        // }

        void IDragHandler.OnDrag(PointerEventData data)
        {
            if (isLocked) return;
            if (expander.isMaximised) expander.MinimiseWindow();
            // Apply movement
            var newPos = parentWindow.anchoredPosition + data.delta / canvas.scaleFactor;

            // Get parent rect (the area we want to stay inside)
            var parentRect = parentWindow.parent as RectTransform;

            // Half sizes for clamping
            var halfSize = parentWindow.sizeDelta * 0.5f;
            var parentHalfSize = parentRect.rect.size * 0.5f;

            // Clamp so the window stays fully inside
            var clampedX = Mathf.Clamp(newPos.x, -parentHalfSize.x + halfSize.x, parentHalfSize.x - halfSize.x);
            var clampedY = Mathf.Clamp(newPos.y, -parentHalfSize.y + halfSize.y, parentHalfSize.y - halfSize.y);

            parentWindow.anchoredPosition = new Vector2(clampedX, clampedY);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (isLocked) return;
            // isGrabbed = false;
            if (onWindowMoved != null) onWindowMoved.Invoke();
        }

        /// <summary>
        ///     Toggle interactivity of handle
        /// </summary>
        /// <param name="input">is interactive</param>
        public void SetIsLocked(bool input)
        {
            isLocked = input;
            // isGrabbed = false;
        }

        /// <summary>
        ///     Trigger that window has started to be moved
        /// </summary>
        public void SetIsGrabbed()
        {
            mouseOffset = parentWindow.position - Input.mousePosition;
            // isGrabbed = true;
            parentWindow.SetAsLastSibling();
        }
    }
}