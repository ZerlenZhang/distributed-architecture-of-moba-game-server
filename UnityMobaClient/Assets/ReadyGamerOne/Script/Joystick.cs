using ReadyGamerOne.Global;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyGamerOne.Script
{
    public class Joystick : UnityEngine.MonoBehaviour,IDragHandler,IEndDragHandler
    {
        public Transform stick;
        public float maxR;
        private Vector2 touchDir=Vector2.zero;

        /// <summary>
        /// 触摸方向
        /// </summary>
        public Vector2 TouchDir => touchDir;
        
        private void Start()
        {
            this.stick.localPosition=Vector3.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos=Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.transform as RectTransform,
                eventData.position,
                GlobalVar.G_Canvas.GetComponent<Canvas>().worldCamera, out pos);

            
            var len = pos.magnitude;
            if (len > maxR)
            {
                pos = pos.normalized * maxR;
            }

            this.touchDir.x = pos.x / maxR;
            this.touchDir.y = pos.y / maxR;
            
            this.stick.localPosition = pos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.stick.localPosition=Vector3.zero;
            touchDir=Vector2.zero;
        }
    }
}