using UnityEngine;
using UnityEngine.Events;

namespace Rellac.Windows
{
    /// <summary>
    ///     Script to handle pull handles to expand the windows
    /// </summary>
    public class GUIWindowHandle : GUIPointerObject
    {
        /// <summary>
        ///     Window to apply pull effect to
        /// </summary>
        [Tooltip("Window to apply pull effect to")] [SerializeField]
        private RectTransform parentWindow;

        /// <summary>
        ///     Handle is locked and unusable
        /// </summary>
        [Tooltip("Handle is locked and unusable")] [SerializeField]
        private bool isLocked;

        /// <summary>
        ///     Minimum width parent window can be set to
        /// </summary>
        [Tooltip("Minimum width parent window can be set to")] [SerializeField]
        private float minWidth = 50;

        /// <summary>
        ///     Minimum height parent window can be set to
        /// </summary>
        [Tooltip("Minimum height parent window can be set to")] [SerializeField]
        private float minHeight = 50;

        /// <summary>
        ///     Sprite to show for cursor when this handle is highlighted
        /// </summary>
        [Tooltip("Sprite to show for cursor when this handle is highlighted")] [SerializeField]
        private Texture2D cursor;

        /// <summary>
        ///     Directional axis to pull window with this handle
        /// </summary>
        [Tooltip("Directional axis to pull window with this handle")] [SerializeField]
        private Axis axis = Axis.Horizontal;

        /// <summary>
        ///     Fired when user pulls on the handle
        /// </summary>
        public UnityEvent onWindowPulled;

        private Canvas canvas;
        private RectTransform canvasRect;
        private Direction direction;


        private Vector2 initialMousePos;
        private Vector2 initialPivot;
        private Vector2 initialSize;
        private bool isGrabbed;

        private void Start()
        {
            canvas = LaptopUIWindow.GetCanvas(transform.parent);
            canvasRect = canvas.GetComponent<RectTransform>();
            //register to pointer events
            onPointerDown.AddListener(SetIsGrabbed);
            onPointerDown.AddListener(parentWindow.SetAsLastSibling);
            onPointerEnter.AddListener(ShowCursor);
            onPointerExit.AddListener(ResetCursor);

            // find what direction we're pulling with this handle
            switch (axis)
            {
                case Axis.Horizontal:
                    if (transform.position.x > parentWindow.position.x)
                        direction = Direction.Right;
                    else
                        direction = Direction.Left;
                    break;

                case Axis.Vertical:
                    if (transform.position.y > parentWindow.position.y)
                        direction = Direction.Up;
                    else
                        direction = Direction.Down;
                    break;

                case Axis.Diagonal:
                    if (transform.position.y > parentWindow.position.y)
                    {
                        if (transform.position.x > parentWindow.position.x)
                            direction = Direction.UpRight;
                        else
                            direction = Direction.UpLeft;
                    }
                    else
                    {
                        if (transform.position.x > parentWindow.position.x)
                            direction = Direction.DownRight;
                        else
                            direction = Direction.DownLeft;
                    }

                    break;
            }
        }

        private void Update()
        {
            if (!isGrabbed)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                isGrabbed = false;
                parentWindow.SetPivot(initialPivot);
                onWindowPulled?.Invoke();
                return;
            }

            var mouseDelta = ((Vector2)Input.mousePosition - initialMousePos) / canvas.scaleFactor;
            var size = initialSize;

            switch (direction)
            {
                case Direction.Up:
                    size += new Vector2(0, mouseDelta.y);
                    break;
                case Direction.Down:
                    size -= new Vector2(0, mouseDelta.y);
                    break;
                case Direction.Left:
                    size -= new Vector2(mouseDelta.x, 0);
                    break;
                case Direction.Right:
                    size += new Vector2(mouseDelta.x, 0);
                    break;
                case Direction.UpRight:
                    size += new Vector2(mouseDelta.x, mouseDelta.y);
                    break;
                case Direction.UpLeft:
                    size += new Vector2(-mouseDelta.x, mouseDelta.y);
                    break;
                case Direction.DownRight:
                    size += new Vector2(mouseDelta.x, -mouseDelta.y);
                    break;
                case Direction.DownLeft:
                    size += new Vector2(-mouseDelta.x, -mouseDelta.y);
                    break;
            }

            // Clamp to minimum size
            size.x = Mathf.Max(size.x, minWidth);
            size.y = Mathf.Max(size.y, minHeight);

            // // Clamp to canvas bounds
            //
            // // Max size so window stays inside canvas
            // var maxWidth = canvasRect.rect.width;
            // var maxHeight = canvasRect.rect.height;
            //
            // size.x = Mathf.Min(size.x, maxWidth);
            // size.y = Mathf.Min(size.y, maxHeight);

            parentWindow.sizeDelta = size;
        }


        /// <summary>
        ///     Toggle interactivity of handle
        /// </summary>
        /// <param name="input">is interactive</param>
        public void SetIsLocked(bool input)
        {
            isLocked = input;
            parentWindow.SetAsLastSibling();
        }

        /// <summary>
        ///     Trigger that this handle has been grabbed
        /// </summary>
        public void SetIsGrabbed()
        {
            if (isLocked) return;
            isGrabbed = true;

            initialMousePos = Input.mousePosition;
            initialSize = parentWindow.sizeDelta;
            initialPivot = parentWindow.pivot;

            // Set Pivot to correct value based on direction so we don't need to offset movement
            switch (direction)
            {
                case Direction.Up:
                    parentWindow.SetPivot(new Vector2(0.5f, 0));
                    break;
                case Direction.Down:
                    parentWindow.SetPivot(new Vector2(0.5f, 1));
                    break;
                case Direction.Left:
                    parentWindow.SetPivot(new Vector2(1, 0.5f));
                    break;
                case Direction.Right:
                    parentWindow.SetPivot(new Vector2(0, 0.5f));
                    break;
                case Direction.UpRight:
                    parentWindow.SetPivot(new Vector2(0, 0));
                    break;
                case Direction.UpLeft:
                    parentWindow.SetPivot(new Vector2(1, 0));
                    break;
                case Direction.DownRight:
                    parentWindow.SetPivot(new Vector2(0, 1));
                    break;
                case Direction.DownLeft:
                    parentWindow.SetPivot(new Vector2(1, 1));
                    break;
            }

            parentWindow.SetAsLastSibling();
        }

        /// <summary>
        ///     Show the changed cursor when this handle is highlighted
        /// </summary>
        public void ShowCursor()
        {
            if (!isLocked && cursor != null) Cursor.SetCursor(cursor, new Vector2(16, 16), CursorMode.Auto);
        }

        /// <summary>
        ///     Return the cursor to the default state
        /// </summary>
        public void ResetCursor()
        {
            if (cursor != null) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private enum Axis
        {
            Horizontal,
            Vertical,
            Diagonal
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }
    }
}