using UnityEngine;
using UnityEngine.UI;

namespace ReadyGamerOne.Utility
{
    public static class AlphaExtension
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            var okAlpha = Mathf.Clamp(alpha, 0, 1);
            return new Color(color.r, color.g, color.b, okAlpha);

        }

        public static void SetAlpha(this Image image, float alpha)
        {
            
            var okAlpha = Mathf.Clamp(alpha, 0, 1);
            var color = image.color;
            color = new Color(color.r, color.g, color.b, okAlpha);
            image.color = color;
        }
        
    }
}