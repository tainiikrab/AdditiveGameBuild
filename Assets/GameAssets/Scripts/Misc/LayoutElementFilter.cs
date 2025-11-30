using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Wappen.UI
{
    /// <summary>
    /// Similar to LayoutElement but with limiter.
    /// </summary>
    [AddComponentMenu("Layout/Layout Element Filter", 141)]
    [ExecuteInEditMode]
    public class LayoutElementFilter : LayoutElementBehaviour
    {
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }

        [Tooltip("Must implement ILayoutElement.")] [SerializeField]
        private Component m_SourceElement = default;

        [SerializeField] protected FitMode m_HorizontalFit = FitMode.Unconstrained;

        [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;

        [Header("Filtering")] [Tooltip("Max width.")] [SerializeField]
        protected float m_MaxPreferredWidth = 0;

        private float m_PreferredWidth;
        private float m_PreferredHeight;
        public override float preferredWidth => m_PreferredWidth;
        public override float preferredHeight => m_PreferredHeight;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (m_SourceElement != null && !(m_SourceElement is ILayoutElement))
            {
                // When drag and drop, m_SourceElement could get RectTransform instead
                // Make sure to reacquire if ILayoutElement is avilable
                var nextAvailable = m_SourceElement.GetComponent<ILayoutElement>();
                if (nextAvailable != null && nextAvailable is Component c)
                {
                    // Helper.RecordUndo(this, "m_SourceElement route");
                    m_SourceElement = c; // Change to that instead
                }
                else
                {
                    // Debug.LogWarning(
                    //     $"{this.GetDebugPath()} requires m_SourceElement to be component with ILayoutElement.", this);
                }
            }

            base.OnValidate();
        }
#endif
        private void HandleFittingAlongAxis(int axis)
        {
            var fitting = axis == 0 ? m_HorizontalFit : m_VerticalFit;
            var ax = (RectTransform.Axis)axis;
            var elm = m_SourceElement as ILayoutElement;

            if (fitting == FitMode.Unconstrained || elm == null)
            {
                if (ax == RectTransform.Axis.Horizontal)
                    m_PreferredWidth = -1;
                else
                    m_PreferredHeight = -1;
                return;
            }

            // Set size to min or preferred size
            float s;
            if (fitting == FitMode.MinSize)
            {
                if (ax == RectTransform.Axis.Horizontal)
                    s = elm.minWidth;
                else
                    s = elm.minHeight;
                //s = LayoutUtility.GetMinSize( r, axis );
            }
            else
            {
                if (ax == RectTransform.Axis.Horizontal)
                    s = elm.preferredWidth;
                else
                    s = elm.preferredHeight;
                //s = LayoutUtility.GetPreferredSize( r, axis );
            }

            // Apply limit
            if (axis == 0 && m_MaxPreferredWidth > 0) // Horz
                s = Mathf.Clamp(s, 0, m_MaxPreferredWidth);

            if (ax == RectTransform.Axis.Horizontal)
                m_PreferredWidth = s;
            else
                m_PreferredHeight = s;
        }

        public override void CalculateLayoutInputHorizontal()
        {
            HandleFittingAlongAxis(0);
        }

        public override void CalculateLayoutInputVertical()
        {
            HandleFittingAlongAxis(1);
        }
    }

    /// <summary>
    /// Base class for class that want to control ILayoutElement.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class LayoutElementBehaviour : UIBehaviour, ILayoutElement
    {
        /* ILayoutElement //////////////////////////////////*/

        public virtual float minWidth => -1;

        public virtual float preferredWidth => -1;

        public virtual float minHeight => -1;

        public virtual float preferredHeight => -1;

        public virtual float flexibleWidth => -1;

        public virtual float flexibleHeight => -1;

        [SerializeField] private int m_LayoutPriority = 2;
        public int layoutPriority => m_LayoutPriority;

        public abstract void CalculateLayoutInputHorizontal();

        public abstract void CalculateLayoutInputVertical();

        /* Unity ///////////////////////////////////////*/

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif

        /* Internal ///////////////////////////////////////////*/

        [System.NonSerialized] private RectTransform m_Rect;

        protected RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        /* Dirtying stuff //////////////////////*/

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnDisable()
        {
            SetDirty();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    }
}