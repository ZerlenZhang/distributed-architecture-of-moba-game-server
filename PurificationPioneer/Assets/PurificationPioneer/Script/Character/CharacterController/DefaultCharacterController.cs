using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class DefaultCharacterController : 
        PpCharacterController<DefaultAnimator>
    {
        public Transform shotPoint;
        public float attackMinDeltaTime = 0.5f;
        private float lastAttackTime = 0;
        public int weaponId = 0;
        public WeaponConfigAsset WeaponConfig { get; protected set; }

        protected override void InitCharacter()
        {
            base.InitCharacter();
            lastAttackTime = Time.realtimeSinceStartup;
            WeaponConfig = ResourceMgr.Instantiate<WeaponConfigAsset>(
                AssetConstUtil.GetWeaponConfigKey(weaponId));
        }

        protected override void OnAttack()
        {
            base.OnAttack();
            var currentTime = Time.realtimeSinceStartup;
            if (currentTime - lastAttackTime > attackMinDeltaTime)
            {
                lastAttackTime = currentTime;
                //attack
                var shotDir = shotPoint.forward;
                WeaponConfig.CommonAttack(shotPoint.position, shotDir);
            }

        }
    }
}