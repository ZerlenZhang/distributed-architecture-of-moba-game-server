using System;
using System.Collections;
using System.Collections.Generic;
using ReadyGamerOne.Common;
using ReadyGamerOne.View.Effects;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyGamerOne.View
{
    /// <summary>
    /// 总UI管理者
    /// </summary>
    public static class PanelMgr
    {
        public static event Action onBeforeClear;

        public static IStackPanel CurrentPanel
        {
            get
            {
                if (panelStack.Count == 0)
                    return null;
                return panelStack.Peek();
            }
        }
        
        
        #region 加载Panel
        
        /// <summary>
        /// 加载一个新面板
        /// </summary>
        /// <param name="name"></param>
        public static void PushPanel(string name)
        {
            if (panelStack.Count != 0)
                panelStack.Peek().Disable();

            var panel = AbstractPanel.GetPanel(name);
            panel.Enable();
            panelStack.Push(panel);
//            Debug.Log("入栈：" + panelStack.Count + " name:" + name);
        }

        /// <summary>
        /// 加载新面板，带动画
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="transition"></param>
        public static void PushPanel(string panelName, AbstractTransition transition)
        {
            var panel = AbstractPanel.GetPanel(panelName);
            transition.PushPanel(panel);
            panelStack.Push(panel);

//            Debug.Log("入栈：" + panelStack.Count + " name:" + panelName);
        }

        /// <summary>
        /// 加载新面板，带效果
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="abstractScreenEffect"></param>
        public static void PushPanel(string panelName, AbstractScreenEffect abstractScreenEffect)
        {
            var panel = AbstractPanel.GetPanel(panelName);
            abstractScreenEffect.OnBegin(CurrentPanel as AbstractPanel, panel);
            panelStack.Push(panel);
        }

        /// <summary>
        /// 加载新面板，都带
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="transition"></param>
        /// <param name="abstractScreenEffect"></param>
        public static void PushPanel(string panelName, AbstractTransition transition,
            AbstractScreenEffect abstractScreenEffect)
        {
            var panel = AbstractPanel.GetPanel(panelName);
            transition.onBegin += abstractScreenEffect.OnBegin;
            transition.PushPanel(panel);
            panelStack.Push(panel);
        }
        

        #region 加载Panel，同时广播一条消息
        public static void PushPanelWithMessage(string panelName, string message)
        {
            PushPanel(panelName);
            CEventCenter.BroadMessage(message);
        }

        public static void PushPanelWithMessage<T>(string panelName, string message, T arg1)
        {
            PushPanel(panelName);
            CEventCenter.BroadMessage<T>(message,arg1);
        }

        public static void PushPanelWithMessage<T1, T2>(string panelName, string message, T1 arg1, T2 arg2)
        {
            PushPanel(panelName);
            CEventCenter.BroadMessage(message, arg1, arg2);
        }
        

        #endregion
        
        
        #endregion


        #region 移除Panel

        /// <summary>
        /// 移除当前面板
        /// </summary>
        public static void PopPanel()
        {
            //Debug.Log("1出栈：" + this.panelStack.Count + " name:" +CurrentPanel.InternalPanelName);
            panelStack.Peek().Destory();
            //Debug.Log("2出栈：" + this.panelStack.Count + " name:" + CurrentPanel.InternalPanelName);
            panelStack.Pop();
            if (panelStack.Count > 0)
                panelStack.Peek().Enable();
        }        

        #endregion
        

        /// <summary>
        /// 清空Panel栈
        /// </summary>
        public static void Clear()
        {
            onBeforeClear?.Invoke();
            while (panelStack.Count>0)
            {
                panelStack.Pop().Destory();
            }
        }

        
        #region Effects
        
        /// <summary>
        /// 淡出效果，全屏幕逐渐变为 Color颜色
        /// </summary>
        /// <param name="time"></param>
        /// <param name="color"></param>
        /// <param name="callBack"></param>
        public static Coroutine FadeOut(float time, Color color,Action callBack=null)
        {
            var image = GetEffectImage();
            image.color = new Color(color.r, color.g, color.b, 0);
            return image.StartCoroutine(ChangeAlpha(image, 1, time, callBack));
        }

        /// <summary>
        /// 淡入效果，全屏幕渐变为正常色
        /// </summary>
        /// <param name="time"></param>
        /// <param name="color"></param>
        /// <param name="callBack"></param>
        public static Coroutine FadeIn(float time,  Action callBack=null)
        {
            var image = GetEffectImage();
            return image.StartCoroutine(ChangeAlpha(image, 0, time, callBack));
        }
        

        #endregion
        


        
        
        #region Private

        
        private static Stack<IStackPanel> panelStack = new Stack<IStackPanel>();
        private static IEnumerator ChangeAlpha(Image image, int targetAlpha, float time, Action callBack)
        {
             //Time.deltaTime/time
             var rowColor = image.color;
             for (var timer = 0f; timer < time; timer += Time.deltaTime)
             {
                 if(targetAlpha==0)
                    image.color=new Color(rowColor.r,rowColor.g,rowColor.b,1-timer/time);
                 else
                     image.color=new Color(rowColor.r,rowColor.g,rowColor.b,timer/time);
                 yield return null;

             }    
             callBack?.Invoke();
        }


        private static Image GetEffectImage()
        {
            var effectPanel = Global.GlobalVar.G_Canvas.transform.Find("EffectPanel");
            if (effectPanel == null)
            {
                var panelObj = new GameObject("EffectPanel");
                panelObj.transform.SetParent(Global.GlobalVar.G_Canvas.transform);
                panelObj.transform.localPosition = Vector3.zero;
                var image = panelObj.AddComponent<Image>();
                var rect = panelObj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(Screen.width, Screen.height);
                rect.ForceUpdateRectTransforms();
                
                return image;
            }

            if (effectPanel.gameObject.activeSelf == false)
                effectPanel.gameObject.SetActive(true);
            
            effectPanel.SetAsLastSibling();

            return effectPanel.GetComponent<Image>();
        }
        
        #endregion

        
        

    }
}
