using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ReadyGamerOne.Script
{
    public class UIInputer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
        IPointerDownHandler, IPointerClickHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
    {
        public event Action<PointerEventData> eventOnPointerEnter;
        public event Action<PointerEventData> eventOnPointerExit;
        public event Action<PointerEventData> eventOnPointerUp;
        public event Action<PointerEventData> eventOnPointerDown;
        public event Action<PointerEventData> eventOnPointerClick;
        public event Action<PointerEventData> eventOnBeginDrag;
        public event Action<PointerEventData> eventOnEndDrag;
        public event Action<PointerEventData> eventOnDrag;

        public event Action<GameObject> eventOnClickObj;

        private void Update()
        {
            if (eventOnClickObj == null)
                return;
            //如果鼠标按下
            if (Input. GetMouseButtonDown(0))
            {
                //获取碰撞信息
                var hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                
                //如果点在某个物体上
                if (hit.Length>=1)
                {
                    //如果点击的物体是自己，可以开始拖拽！
                    if (hit[0].transform == this.transform)
                        eventOnClickObj(hit[0].transform.gameObject);
                    else
                    {
                        Debug.Log("点击的是：" + hit[0].transform.name + "，不是" + gameObject.name);
                    }
                }
                else
                {
                    Debug.Log("没有点击物体");
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            eventOnPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            eventOnPointerExit?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            eventOnPointerUp?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            eventOnPointerDown?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            eventOnPointerClick?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            eventOnBeginDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            eventOnEndDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            eventOnDrag?.Invoke(eventData);
        }
    }
}
