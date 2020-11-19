using System;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class UiUtil
    {
        /// <summary>
        /// 世界坐标转UI坐标
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="uiCanvas"></param>
        /// <param name="uiCamera"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Vector2 WorldPointToAnchoredPosition(Vector3 worldPos,Canvas uiCanvas=null,  Camera uiCamera = null)
        {
            var camera = uiCamera ?? Camera.main;
            if (!camera)
                throw new Exception("Camera is null");

            var canvas = uiCanvas ?? ReadyGamerOne.Global.GlobalVar.GCanvasObj?.GetComponent<Canvas>();
            if (!canvas)
                throw new Exception("Canvas is null");

            //得到画布的尺寸
            var uisize = canvas.GetComponent<RectTransform>().sizeDelta;

            //将世界坐标转换为屏幕坐标
            var screenpos = camera.WorldToScreenPoint(worldPos);

            //转换为以屏幕中心为原点的屏幕坐标
            var screenpos2 = new Vector2(
                screenpos.x - (Screen.width / 2),
                screenpos.y - (Screen.height / 2));

            //得到UGUI的anchoredPosition
            return new Vector2(
                (screenpos2.x / Screen.width) * uisize.x,
                (screenpos2.y / Screen.height) * uisize.y);
        }
    }
}