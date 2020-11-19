using ReadyGamerOne.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ReadyGamerOne.ScriptableObjects
{
    public class AnimationCurveAsset:ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("ReadyGamerOne/Create/AnimationCurveAsset")]
        public static void CreateAsset()
        {
            EditorUtil.CreateAsset<AnimationCurveAsset>("AnimationCurveAsset");
        }         
#endif
        
        
        public AnimationCurve curve;
    }
}