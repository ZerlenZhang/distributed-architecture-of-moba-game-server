using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    public abstract class SkillConfigAsset : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private string 备注;

        [Space]
#endif
        public int temp;

        /// <summary>
        /// 一次性释放
        /// </summary>
        public abstract void Apply(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null,
        Transform target = null);
    }
}