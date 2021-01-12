using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    public abstract class WeaponConfigAsset : ScriptableObject,IPpWeapon
    {
#if UNITY_EDITOR
        [SerializeField] private string 备注;
        [Space]
#endif
        public int weaponId;
        [Header("攻击百分比加成")]
        public int attackPercent;
        [Header("喷涂效率百分比加成")]
        public int paintPercent;
        
        [Header("平A")]
        [SerializeField]protected SkillConfigAsset commonAttack;
        
        [Header("一技能")]
        [SerializeField]protected SkillConfigAsset firstSkill;
        [Header("二技能")]
        [SerializeField]protected SkillConfigAsset secondSkill;

        [Header("世界观故事")]
        public string story0;
        public string story1;

        public abstract void CommonAttack(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null,
            Transform target = null);

        public abstract void FirstSkill(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null,
            Transform target = null);

        public abstract void SecondSkill(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null,
            Transform target = null);
    }
}