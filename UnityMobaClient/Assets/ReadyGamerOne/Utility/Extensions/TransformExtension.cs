using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class TransformExtension
    {
        /// <summary>
        /// 获取一个坐标在某一个Transform视角里的位置，x和y范围都是（-2，2）
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static Vector2 GetViewVector2(this Transform transform, Vector3 targetPos)
        {
            var dir = targetPos - transform.position;

            var dirHor = new Vector3(dir.x, 0, dir.z);
            var dirVer=new Vector3(0,dir.y,Mathf.Sqrt(dir.x*dir.x+dir.z*dir.z));
            
            var isRight = Vector3.Cross(dirHor, transform.forward).y<0;
            var horDegree = Vector3.Angle(dirHor, transform.forward);
            var horValue = ( isRight ? 1 : -1 )* horDegree / 90;

            var verticalDegree = Vector3.Angle(dirVer, transform.forward);
            var verValue = (targetPos.y > transform.position.y ? 1 : -1) * verticalDegree / 90.0f;

            return new Vector2(horValue, verValue);
        }
    }
}