using UnityEngine;
using UnityEngine.EventSystems;

namespace Rellac.Windows
{
    public class GUIWindow : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private int startWidth = 800;
        [SerializeField] private int startHeight = 500;
        private OpenWindowButton initializer;

        private void Awake()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth, startHeight);
        }

        /// <summary>
        ///     Detect right-click and close
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right) CloseWindow();
        }

        /// <summary>
        ///     Close window by destroying this GameObject
        /// </summary>
        public void CloseWindow()
        {
            if (initializer) initializer.isWindowOpen = false;
            Destroy(gameObject);
        }

        public void Initialize(OpenWindowButton initializer)
        {
            this.initializer = initializer;
        }
    }
}