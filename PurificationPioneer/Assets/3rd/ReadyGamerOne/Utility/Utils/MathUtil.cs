using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class MathUtil
    {
        /// <summary>
        /// 正四面体四个顶点方向向量。一个顶点在x轴，一条棱垂直x轴
        /// </summary>
        public static readonly Vector3[] RegularTetrahedronVertexDir;
        static MathUtil()
        {
            var bottomY = -1 / (2 * Mathf.Sqrt(2));
            var halfSide = Mathf.Sqrt(3) / 2;
            RegularTetrahedronVertexDir = new[]
            {
                Vector3.up,
                new Vector3(1, bottomY, 0).normalized,
                new Vector3(-0.5f, bottomY, halfSide).normalized,
                new Vector3(-0.5f, bottomY, -halfSide).normalized
            };
        }
    }
}