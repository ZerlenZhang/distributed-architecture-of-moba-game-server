using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ReadyGamerOne.Global
{
    public partial class GlobalVar
    {
        public static event Action<Canvas> onCreateCanvas; 
        public static GameObject GCanvasObj
        {
            get
            {
                if (canvasObj == null)
                {
                    var c = Object.FindObjectOfType<Canvas>();
                    if (c)
                    {
                        canvasObj = c.gameObject;
                    }
                    else
                    {
                        var canvasObj = new GameObject("GlobalCanvas");
                        canvasObj.AddComponent<RectTransform>();
                        c = canvasObj.AddComponent<Canvas>();
                        canvasObj.AddComponent<CanvasScaler>();
                        canvasObj.AddComponent<GraphicRaycaster>();
                        c.renderMode = RenderMode.ScreenSpaceOverlay;
                        onCreateCanvas?.Invoke(c);
                        GlobalVar.canvasObj = canvasObj;

                        if (null == Object.FindObjectOfType<EventSystem>())
                        {
                            var es = new GameObject("EventSystem");
                            es.AddComponent<EventSystem>();
                            es.AddComponent<StandaloneInputModule>();
                        }                        
                    }

                    Assert.IsTrue(canvasObj);

                    _canvasCom = canvasObj.GetComponent<Canvas>();
                }

                return canvasObj;
            }            
        }

        public static Canvas CanvasComponent => _canvasCom;
        
        private static GameObject canvasObj;
        private static Canvas _canvasCom;

        public static Vector3 GCanvasButton => GCanvasObj.transform.position - new Vector3(0, Screen.height, 0);

        public static Vector3 GCanvasTop => GCanvasObj.transform.position + new Vector3(0, Screen.height, 0);

        public static Vector3 GCanvasLeft => GCanvasObj.transform.position - new Vector3(Screen.width, 0, 0);

        public static Vector3 GCanvasRight => GCanvasObj.transform.position + new Vector3(Screen.width, 0, 0);
    }
}
