using PurificationPioneer.Const;
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
        public int bulletId = 1;

        protected override void InitCharacter()
        {
            base.InitCharacter();
            lastAttackTime = Time.realtimeSinceStartup;
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
                var directBullet = ResourceMgr.InstantiateGameObject(
                    BulletName.DirectBullet).GetComponent<DirectionFrameSyncBullet>();
                directBullet.Init(1, shotPoint.position, shotDir);
            }

        }
    }
}