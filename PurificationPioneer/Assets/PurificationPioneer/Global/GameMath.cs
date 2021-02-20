using Es.InkPainter;
using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.Global
{
    public static class GameMath
    {
        public static float CalculateScale(GameObject go, Brush brush)
        {
            var sqr = go.transform.lossyScale.x; //Mathf.Pow(go.transform.lossyScale.x, 2);
            return brush.Scale / sqr * GameSettings.Instance.BrushScaler;
        }
    }
}