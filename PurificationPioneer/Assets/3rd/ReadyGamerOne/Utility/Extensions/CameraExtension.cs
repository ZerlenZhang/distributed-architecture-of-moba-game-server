using System.Collections.Generic;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class CameraExtension
    {
        /// <summary>
        /// 获取camera在一定距离前四个角世界坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static List<Vector3> GetCorners(this Camera camera, float distance)
        {
            var theta = camera.fieldOfView/2;

            var halfHeight = distance * Mathf.Tan(Mathf.Deg2Rad * theta);

            var halfWidth = halfHeight * camera.aspect;

            var transform = camera.transform;
            var center = transform.position + transform.forward * distance;

            var right = transform.right;
            var up = transform.up;
            return  new List<Vector3>
            {
                center + right * (-halfWidth) + up * (halfHeight),
                center + right * (halfWidth) + up * (halfHeight),
                center + right * (halfWidth) + up * (-halfHeight),
                center + right * (-halfWidth) + up * (-halfHeight)
            };


        }
    }
}