using PurificationPioneer.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Scriptable
{
    [CreateAssetMenu(fileName = "NewGunWeaponAsset", menuName = "净化先锋/Weapon/GunWeapon")]
    public class GunWeapon:WeaponConfigAsset
    {
        public override void CommonAttack(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null)
        {
            commonAttack?.Apply(createPos, direction, targetPoint, target);
        }

        public override void FirstSkill(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null)
        {
            throw new System.NotImplementedException();
        }

        public override void SecondSkill(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null)
        {
            throw new System.NotImplementedException();
        }


    }
}