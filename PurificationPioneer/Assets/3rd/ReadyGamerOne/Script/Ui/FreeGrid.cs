using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Script
{
    public class FreeGrid : MonoBehaviour
    {
        [Header("控制四周边距")]
        public RectOffset padding;

        [Header("控制子物体大小")]
        public bool controlChildrenSize;
        public Vector2 childSize = Vector2.one * 100;
        
        [Header("子物体Rect控制")]
        public Vector2 pivot = new Vector2(0, 1);
        public Vector2 anchorMin=new Vector2(0,1);
        public Vector2 anchorMax=new Vector2(0,1);
        
        [Header("控制子物体间距")]
        public Vector2 space;
        
        [Header("控制子物体行数")]
        public int rowCount = 4;


        private RectTransform selfRect;
        protected virtual void Awake()
        {
            selfRect = GetComponent<RectTransform>();
            Assert.IsTrue(selfRect);
        }
        
        protected virtual void Start()
        {
            ReBuild();
        }
        

        [ContextMenu("Sort")]
        public void ReBuild()
        {
            var list = new List<RectTransform>();
            for (var i = 0; i < selfRect.childCount; i++)
            {
                list.Add(selfRect.GetChild(i).GetComponent<RectTransform>());
            }

            var index = 0;
            foreach (var childRect in list)
            {
                if (!_CheckIfSort(childRect))
                {
                    SetChildState(childRect, false);
                    continue;
                }

                SetChildState(childRect, true);
                
                //设置锚点
                childRect.pivot = pivot;
                childRect.anchorMin = anchorMin;
                childRect.anchorMax = anchorMax;
                
                //设置大小
                if (controlChildrenSize)
                    childRect.sizeDelta = childSize;
                
                //设置位置
                var row = index % rowCount;
                var col = index / rowCount;

                var childRectSize = childRect.sizeDelta;
                var offset=  new Vector2(
                    padding.left + col*(childRectSize.x+space.x),
                    - (padding.top + row*(childRectSize.y+space.y)));

                SetPosition(childRect, offset);

                index++;
            }
        }
        
        protected virtual void SetChildState(RectTransform child, bool state)
        {
            child.gameObject.SetActive(state);
        }


        /// <summary>
        /// 外部可以赋值这个委托，进而重写CheckIfSort的操作
        /// </summary>
        public Func<RectTransform, bool> CheckIfSort { set; private get; } = null;

        /// <summary>
        /// 内部使用这个，优先采用CheckIfSort，否则采用默认方法
        /// </summary>
        private Func<RectTransform, bool> _CheckIfSort => CheckIfSort ?? _internal_check_if_sort_;
        
        private bool _internal_check_if_sort_(RectTransform childRect)
        {
            return childRect.gameObject.activeSelf;
        }

        protected virtual void SetPosition(RectTransform childRect, Vector2 targetAnchoredPosition)
        {
            childRect.anchoredPosition = targetAnchoredPosition;
        }
    }
}