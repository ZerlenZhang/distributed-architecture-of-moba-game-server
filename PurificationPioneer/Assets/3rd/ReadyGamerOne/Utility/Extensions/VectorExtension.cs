using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class VectorExtension
    {
        public static Vector2 ToVector2(this Vector3 self)
        {
            return new Vector2(self.x, self.y);
        }

        public static Vector3 ToVector3(this Vector2 self, float zValue = 0f)
        {
            return new Vector3(self.x, self.y, zValue);
        }
        
        /// <summary>
        /// 判断两个Vector是否在容错范围内相等
        /// </summary>
        /// <param name="self"></param>
        /// <param name="another"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool EqualsTo(this Vector3 self, Vector3 another, float tolerance = 0.01f)
        {
            return Mathf.Abs(self.x - another.x) < tolerance
                   && Mathf.Abs(self.y - another.y) < tolerance
                   && Mathf.Abs(self.z - another.z) < tolerance;
        }

        public static bool EqualsTo(this Vector2 self, Vector2 another, float tolerance = 0.01f)
        {
            return Mathf.Abs(self.x - another.x) < tolerance
                   && Mathf.Abs(self.y - another.y) < tolerance;
        }
        
        /// <summary>
        /// 将一个Vector3向量的XY分量旋转角度制degree
        /// </summary>
        /// <param name="self"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector3 RotateDegree(this Vector3 self, float degree)
        {
            var sin = Mathf.Sin(degree * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degree * Mathf.Deg2Rad);
            
            return new Vector3(
                self.x*cos -self.y*sin,
                self.x*sin+self.y*cos,
                self.z
                );
        }

        /// <summary>
        /// 交换x和y
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector2 Switch(this Vector2 self)
        {
            return new Vector2(self.y, self.x);
        }


        public static Vector2 RotateDegree(this Vector2 self, float degree)
        {
            var sin = Mathf.Sin(degree * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degree * Mathf.Deg2Rad);
            
            return new Vector2(
                self.x*cos -self.y*sin,
                self.x*sin+self.y*cos
            );
        }
    }
}