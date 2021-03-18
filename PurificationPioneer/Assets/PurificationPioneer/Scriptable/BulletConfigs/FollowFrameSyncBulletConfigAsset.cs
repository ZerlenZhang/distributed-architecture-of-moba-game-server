using PurificationPioneer.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Scriptable
{
    [CreateAssetMenu(fileName = "NewFollowFrameSyncBulletConfig", menuName = "净化先锋/Bullet/FollowFrameSyncBulletConfig", order = 0)]
    public class FollowFrameSyncBulletConfigAsset: BulletConfigAsset,IBulletConfig
    {
        public int MaxLife => _maxLife;

        public float MaxSpeed => _maxSpeed;

        public float InitSpeedScale => _initSpeedScale;
        public float MaxVelocityChange => _maxVelocityChange;
        private float _initSpeedScale;
        private int _maxLife;
        private float _maxSpeed;
        private float _maxVelocityChange;
        
        
        public FollowFrameSyncBullet InstantiateAndInitialize(Vector3 createPos, Vector3 direction, Transform target)
        {
            var bulletObj = Object.Instantiate(prefab);
            Assert.IsTrue(bulletObj);

            var script = bulletObj.GetComponent<FollowFrameSyncBullet>();
            Assert.IsNotNull(script);

            var bulletState = new FollowFrameSyncBulletState(
                bulletObj.GetComponent<PpRigidbody>(), direction, createPos, target);
            
            script.Initialize(this, bulletState);
            
            return script;
        }
    }
}