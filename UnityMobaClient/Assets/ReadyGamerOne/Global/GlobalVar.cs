using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ReadyGamerOne.Global
{
    public partial class GlobalVar
    {
        public static event Action<Canvas> onCreateCanvas; 
        public static GameObject G_Canvas
        {
            get
            {
                if (canvas == null)
                {
                    var canvasObj = new GameObject("GlobalCanvas");
                    canvasObj.AddComponent<RectTransform>();
                    var c = canvasObj.AddComponent<Canvas>();
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                    c.renderMode = RenderMode.ScreenSpaceOverlay;
                    onCreateCanvas?.Invoke(c);
                    canvas = canvasObj;

                    if (null == Object.FindObjectOfType<EventSystem>())
                    {
                        var es = new GameObject("EventSystem");
                        es.AddComponent<EventSystem>();
                        es.AddComponent<StandaloneInputModule>();
                    }
                }

                return canvas;
            }            
        }
        
        private static GameObject canvas;

        public static Vector3 GCanvasButton => G_Canvas.transform.position - new Vector3(0, Screen.height, 0);

        public static Vector3 GCanvasTop => G_Canvas.transform.position + new Vector3(0, Screen.height, 0);

        public static Vector3 GCanvasLeft => G_Canvas.transform.position - new Vector3(Screen.width, 0, 0);

        public static Vector3 GCanvasRight => G_Canvas.transform.position + new Vector3(Screen.width, 0, 0);
    }
}
