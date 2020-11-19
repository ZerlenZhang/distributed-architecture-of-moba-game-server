using System;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class GUILayoutUtil
    {
        public static float Slider(string text, float value, float left, float right, GUIStyle style=null,string format="0.00",float sliderLength=100)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{text}\t【", style);
            value =Convert.ToSingle(GUILayout.TextField($"{value.ToString(format)}",30, style));
            value = Mathf.Clamp(value, left, right);
            GUILayout.Label("】", style);
            value = GUILayout.HorizontalSlider(value, left, right, GUILayout.Height(style.fontSize),GUILayout.Width(sliderLength));
            GUILayout.EndHorizontal();
            return value;
        }
    }
}