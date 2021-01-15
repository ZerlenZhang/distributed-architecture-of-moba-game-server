using PurificationPioneer.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Scriptable
{
    [CreateAssetMenu(fileName = "NewDirectionBulletConfig", menuName = "净化先锋/Bullet/DirectionBulletConfig", order = 0)]
    public class DirectionBulletConfigAsset:BulletConfigAsset,IBulletConfig
    {
        #region IBulletConfig

        [SerializeField] private int _maxLife;
        public int MaxLife => _maxLife;
        #endregion
        
        
        [SerializeField] private float _speed;
        public float Speed => _speed;
        [SerializeField] private float _radius;
        public float Radius => _radius;
        [SerializeField] private BrushConfigAsset _brushConfig;   
        public BrushConfigAsset BrushConfig => _brushConfig;        

        
        public DirectionFrameSyncBullet InstantiateAndInitialize(Vector3 createPos, Vector3 direction)
        {
            var bulletObj = Object.Instantiate(prefab);
            Assert.IsTrue(bulletObj);

            var script = bulletObj.GetComponent<DirectionFrameSyncBullet>();
            Assert.IsNotNull(script);
            
            script.Initialize(this, new DirectionBulletState(
                bulletObj.GetComponent<PpRigidbody>(),
                direction,
                createPos));
            
            return script;
        }
    }

}