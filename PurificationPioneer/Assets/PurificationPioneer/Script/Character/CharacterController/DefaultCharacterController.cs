using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class DefaultCharacterController : 
        PpCharacterController<DefaultAnimator>
    {
        public float offsetLen = 1;
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

        protected override void OnAttack(int faceX,int faceY)
        {
            base.OnAttack(faceX, faceY);
            var currentTime = Time.realtimeSinceStartup;
            if (currentTime - lastAttackTime > attackMinDeltaTime)
            {
                lastAttackTime = currentTime;
                //attack
                var shotDir = new Vector3(faceX, 0, faceY).normalized;
                var shotPoint = centerPoint.position + shotDir * offsetLen;
                WeaponConfig.CommonAttack(shotPoint, shotDir);
            }

        }
    }
}