using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Rellac.Windows
{
    public static class GUIWindowUtils
    {
// #if UNITY_EDITOR
// 		[MenuItem("GameObject/UI/Window")]
// 		public static void InstantiateWindow()
// 		{
// 			NewWindow();
// 		}
// #endif

// 		public static GameObject NewWindow()
// 		{
// 			Object mesh = Resources.Load("WindowUIComponent");
// 			Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
// 			if (canvas == null)
// 			{
// 				// Create default canvas
// 				GameObject newCanvas = new GameObject("Canvas");
// 				canvas = newCanvas.AddComponent<Canvas>();
// 				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
// 				newCanvas.AddComponent<CanvasScaler>();
// 				newCanvas.AddComponent<GraphicRaycaster>();
// 				if (GameObject.FindObjectOfType<EventSystem>() == null)
// 				{
// 					// Create defalut EventSystem
// 					new GameObject("EventSystem").AddComponent<EventSystem>(). // create new GameObject with EventSystem
// 						gameObject.AddComponent<StandaloneInputModule>() // add Input Module
// #if UNITY_5_3_OR_NEWER
// 						; // we don't need the Touch Input Module at 5.3+
// #else
// 						.gameObject.AddComponent<TouchInputModule>();
// #endif
// 				}
// 			}
// 			GameObject go = (GameObject)GameObject.Instantiate(mesh);
// 			go.transform.SetParent(canvas.transform);
// 			go.transform.localPosition = Vector2.zero;
// 			go.name = "Window";
// 			return go;
// 		}

        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            var size = rectTransform.rect.size;
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
    }
}