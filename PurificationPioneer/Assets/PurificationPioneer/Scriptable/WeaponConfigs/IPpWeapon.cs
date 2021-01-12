using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    public interface IPpWeapon
    {
        void CommonAttack(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null);
        void FirstSkill(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null);
        void SecondSkill(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null);
    }
}