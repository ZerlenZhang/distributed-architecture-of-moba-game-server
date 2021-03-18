using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Scriptable
{
    public class ShotSeveralFollowBulletSkillAsset:SkillConfigAsset
    {
        public FollowFrameSyncBulletConfigAsset followFrameSyncBulletConfigAsset;
        public override void Apply(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null)
        {
            Assert.IsTrue(target && direction.HasValue);
            var followBullet = 
                followFrameSyncBulletConfigAsset.InstantiateAndInitialize(
                    createPos, direction.Value, target);
        }
    }
}