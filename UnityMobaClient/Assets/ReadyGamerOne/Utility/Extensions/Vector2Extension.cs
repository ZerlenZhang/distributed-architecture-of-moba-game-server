using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class Vector2Extension
    {
        /// <summary>
        /// 将一个vector2向量绕起点旋转一个角度
        /// </summary>
        /// <param name="self"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector2 RotateAngle(this Vector2 self, float degree)
        {
            var alpha = Mathf.Atan2(self.y, self.x);
            var cosAlpha = Mathf.Cos(alpha);
            var sinAlpha = Mathf.Sin(alpha);
            var cosTheta = Mathf.Cos(degree*Mathf.Deg2Rad);
            var sinTheta = Mathf.Sin(degree*Mathf.Deg2Rad);

            var size = self.magnitude;
            
            return new Vector2(
                size * (cosAlpha*cosTheta-sinAlpha*sinTheta),
                size * (sinAlpha*cosTheta+sinTheta*cosAlpha));

        }
    }
}