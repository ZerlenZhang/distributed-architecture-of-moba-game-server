using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    [CreateAssetMenu(fileName = "NewBrushConfig", menuName = "净化先锋/BrushConfig", order = 0)]
    public class BrushConfigAsset : ScriptableObject
    {
        [SerializeField] 
        private Color m_Color = new Color(1f, 0.41f, 0f);
        
        [SerializeField] 
        private float m_Radius = 1;
        
        [SerializeField] 
        private float m_Hardness = 0.5f;
        
        [SerializeField] 
        private float m_Strength = 0.1f;

        public float Strength => m_Strength;
        
        public float Hardness => m_Hardness;

        public float Radius => m_Radius;

        public Color Color => m_Color;
    }


    public static class PaintManagerExtension
    {
        /// <summary>
        /// 使用给定BrushConfig绘制
        /// </summary>
        /// <param name="self"></param>
        /// <param name="p"></param>
        /// <param name="pos"></param>
        /// <param name="brushConfig"></param>
        public static void Paint(this PaintManager self, Paintable p, Vector3 pos, BrushConfigAsset brushConfig)
        {
            self.Paint(p, pos, brushConfig.Radius, brushConfig.Hardness, brushConfig.Strength, brushConfig.Color);
        }
    }
}