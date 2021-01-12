using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    public class BulletConfigAsset : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private string 备注;

        [Space]
#endif
        public GameObject prefab;
    }
}