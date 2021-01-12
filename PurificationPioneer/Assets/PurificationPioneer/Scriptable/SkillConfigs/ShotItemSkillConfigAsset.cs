using PurificationPioneer.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Scriptable
{
    [CreateAssetMenu(fileName = "NewShotItemSkillConfig", menuName = "净化先锋/Skill/ShotItemSkill", order = 0)]
    public class ShotItemSkillConfigAsset:SkillConfigAsset
    {
        public DirectionBulletConfigAsset commonBulletConfig;

        public override void Apply(Vector3 createPos, Vector3? direction = null, Vector3? targetPoint = null, Transform target = null)
        {
            Assert.IsTrue(direction.HasValue);
            var directBullet =
                commonBulletConfig.InstantiateAndInitialize(createPos, direction.Value);
        }
    }
}