using Es.InkPainter;
using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    [CreateAssetMenu(fileName = "NewBrushConfig", menuName = "净化先锋/BrushConfig", order = 0)]
    public class BrushConfigAsset : ScriptableObject
    {
        public Brush brush;
    }
}