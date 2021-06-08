using System;
using System.Collections.Generic;
using ReadyGamerOne.Global;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyGamerOne.Script
{
    public class Dragger : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
    {
        public LayerMask testLayer;
        public bool dragOnce = true;
        
        //设置没有到达目的地会返回初始位置
        public bool backToStart = true;


        private enum DragType
        {
            GameObject,
            Ui
        }
        
        #region 回调事件
        public Action eventOnDrag;

        public Action eventOnBeginDrag;

        public Action eventOnEndDrag;

        public Action<GameObject> eventOnDrop;

        public Action<GameObject> eventOnEnter;

        public Func<GameObject,bool> IsTarget;

        public Action<GameObject> eventOnGetTarget;
        
        
        
        #endregion
 
        #region Private
        
        #region Field
        
        
        //判断是否拖拽过
        private bool dragged = false;
        //是否正在拖拽
        private bool draging = false;

        private GameObject lastGameObject_ui;
        private GameObject lastGameObject_obj;

        //保存开始时候的父物体和本地坐标
        private Vector3 beginLocalPosition;
        private Transform beforeParent;

        private DragType _dragType;

        private RectTransform parentRect;
        private RectTransform selfRect;
        private Vector2 offset;
        
        #endregion
        
        #region Properties
        
        //移动的时候把被拖物体设为不接受UI事件，拖拽结束取消设置，这个列表记录了被修改的UI组件
        private List<Graphic> gcs=new List<Graphic>();
        private bool Raycastable
        {
            set
            {
                if (value)
                {
                    //把修改过的值恢复
                    foreach (var gc in gcs)
                        gc.raycastTarget = true;
                    gcs.Clear();
                }
                else
                {
                    //把物体本身及子物体所有接收UI输入的组件都改成不接受
                    var graphics = GetComponents<Graphic>();
                    foreach (var gc in graphics)
                    {
                        if (gc.raycastTarget)
                        {
                            gc.raycastTarget = false;
                            gcs.Add(gc);
                        }
                    }
                    graphics = GetComponentsInChildren<Graphic>();
                    foreach (var gc in graphics)
                    {
                        if (gc.raycastTarget)
                        {
                            gc.raycastTarget = false;
                            gcs.Add(gc);
                        }
                    }
                }
                
                
            }
        }
        
        #endregion
        
        #region Methods
        
        private void BackToStart()
        {
            transform.SetParent(beforeParent);
            transform.localPosition = beginLocalPosition;
        }
        #endregion
        
        #endregion

        #region MonoBehavior

        private void Start()
        {
            if (this.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                this._dragType = DragType.Ui;
            }
            else
                this._dragType = DragType.GameObject;
        }

        void Update()
        {
            if (!draging)
                return;
            
            //如果鼠标按下
            if (_dragType==DragType.GameObject && Input.GetMouseButtonDown(0))
            {
                //获取碰撞信息
                var hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,testLayer);
                
                //如果点在某个物体上
                if (hit.Length>=1)
                {
                    //如果点击的物体是自己，可以开始拖拽！
                    if(hit[0].transform ==this.transform && !draging)
                        ToStartDrag();
                }
            }
            
            lastGameObject_obj = GetCurrentTarget();
            

            if (_dragType==DragType.GameObject && Input.GetMouseButtonUp(0))
            {
                ToEndDrag();
                
            }
            else
            {
//                print("OnDraging:  " + name);


                #region 判断是否进入某些物体

                 if (lastGameObject_obj)
                     eventOnEnter?.Invoke(lastGameObject_obj);
                 

                #endregion

                switch (_dragType)
                {
                    case DragType.GameObject:
                        var pos = transform.position;
                        var mouseToPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        this.transform.position = new Vector3(mouseToPoint.x, mouseToPoint.y, pos.z);
                        break;
                    case DragType.Ui:
                        var localPoint = Vector2.zero;
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            parentRect,
                            Input.mousePosition,
                            GlobalVar.CanvasComponent.worldCamera,
                            out localPoint))
                        {
                            var parentRectSizeDelta = parentRect.sizeDelta;
                            selfRect.anchoredPosition = offset + localPoint+new Vector2(parentRectSizeDelta.x/2,
                                                            -parentRectSizeDelta.y/2);
                        }
                        
                        
                        break;
                }
                

                
                
                eventOnDrag?.Invoke();
            }
        }        

        #endregion


        private GameObject GetCurrentTarget()
        {
            var hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,testLayer);
        
            if (_dragType == DragType.GameObject && hit.Length>=2)
            {
                return hit[1].transform.gameObject;
            }

            if (_dragType == DragType.Ui && hit.Length >= 1)
            {
                return hit[0].transform.gameObject;
            }

            return null;
        }

        private void ToStartDrag()
        {
//            print("ToStartDrag:  "+transform.name);
            
            //如果之调用一次，并且调用过了，直接返回
            if (dragOnce && dragged)
                return;

            
            //记录开始的父物体和位置
            beforeParent = transform.parent;
            beginLocalPosition = transform.localPosition;

            if (_dragType == DragType.Ui)
            {
                //在UI最上层
                transform.SetParent(GlobalVar.GCanvasObj.transform);  ;
                transform.SetAsLastSibling();     
                
                //设置拖拽物体不接受UI事件
                Raycastable = false;
                
                //记录新父亲和自己的Rect
                selfRect = GetComponent<RectTransform>();
                parentRect = transform.parent.GetComponent<RectTransform>();

                #region 计算offset

                var selfLocalPoint = Vector2.zero;
                var selfScreenPoint = RectTransformUtility.WorldToScreenPoint( 
                    GlobalVar.CanvasComponent.worldCamera, transform.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    selfScreenPoint,
                    GlobalVar.CanvasComponent.worldCamera,
                    out selfLocalPoint);

                var mouseLocalPoint = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    Input.mousePosition,
                    GlobalVar.CanvasComponent.worldCamera,
                    out mouseLocalPoint);

                offset = selfLocalPoint - mouseLocalPoint;
//                Debug.Log($"inOffset:{inOffset}");

                #endregion

            }



            //修改标记
            draging = true;
            
            //回调
            eventOnBeginDrag?.Invoke();
        }

        private void ToEndDrag()
        {
//            Debug.Log("ToEndDrag:  "+transform.name);
            
            //如果只拖拽一次并且已经拖拽过了，就不在执行，直接返回
            if (dragOnce && dragged)
                return;
            
            //设置标记
            draging = false;

            if (_dragType == DragType.Ui)
            {
                //恢复刚刚做得修改
                Raycastable = true;                
            }

            
            //回调
            eventOnEndDrag?.Invoke();

            //是否到达目的地

            GameObject testTarget;
            if(_dragType==DragType.Ui)
                testTarget =  lastGameObject_ui ? lastGameObject_ui : lastGameObject_obj;
            else
            {
                testTarget = GetCurrentTarget();
            }

            if (IsTarget == null)
            {
                Debug.LogWarning("IsTarget is null ???");
            }


            if (IsTarget !=null && testTarget && IsTarget(testTarget))
            {
                //已经到达，那就设置标记——已经拖拽过了
                dragged = true;
                eventOnGetTarget?.Invoke(testTarget);
            }
            else if(backToStart)
            {
                //没到达目的地并且设置了不到达返回起点，那么调用此函数
                BackToStart();
            }
            else
            {
                //就算没到达目的地，位置上不返回起点，父物体也还是要该回去
                transform.SetParent(beforeParent);
            }
        }

        private void OnEnter(GameObject target)
        {
//            print("Dragging " + name + "时，经过：" + inTarget.name);
            eventOnEnter?.Invoke(target);
        }
        
    
        #region drag_Interface
        
        public virtual void  OnBeginDrag(PointerEventData eventData)
        {
            if (draging)
                return;
            ToStartDrag();
        }
        public virtual void   OnEndDrag(PointerEventData eventData)
        {
            ToEndDrag();
        }
        public void OnDrag(PointerEventData eventData)
        {
            this.lastGameObject_ui = eventData?.pointerEnter;

            if(eventData!=null && eventData.pointerEnter && eventData.pointerEnter!=gameObject)
                OnEnter(eventData.pointerEnter);
        }
        
        #endregion

    }
}



