using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Rougelike
{
    public abstract class AbstractEffect<T>:
        IPoolable<T>,
        IResourcableObject
    where T:AbstractEffect<T>,new()
    {
        #region Fields

        private GameObject _gameObject;

        #endregion
        
        #region IResourcableObject

        #region IUnityGameObject

        public GameObject gameObject => _gameObject;
        public Transform transform => _gameObject?.transform;

        public Vector3 position
        {
            get
            {
                Assert.IsNotNull(transform);
                return transform.position;
            }
            set
            {
                Assert.IsNotNull(transform);
                transform.position = value;
            }
        }
        public Vector3 localPosition 
        {
            get
            {
                Assert.IsNotNull(transform);
                return transform.localPosition;
            }
            set
            {
                Assert.IsNotNull(transform);
                transform.localPosition = value;
            }
        }
        #endregion
        
        public abstract string ResPath { get; }
        public virtual void OnInstanciateObject()
        {
            Assert.IsNull(_gameObject);
            _gameObject = ResourceMgr.InstantiateGameObject(ResPath);
        }

        public virtual void DestroyObject()
        {
            if(_gameObject)
                Object.Destroy(_gameObject);
        }
        public virtual void EnableObject()
        {
            if(!_gameObject)
                this.OnInstanciateObject();
            Assert.IsTrue(_gameObject);
            gameObject?.SetActive(true);
        }

        public virtual void DisableObject()
        {
            gameObject?.SetActive(false);
        }
        
        #endregion

        #region IPoolable<AbstractEffect>

        public virtual void OnRecycleToPool()
        {
            DisableObject();
        }

        public virtual void OnGetFromPool()
        {
            EnableObject();
        }        

        #endregion

        protected AbstractEffect()
        {
            this.OnInstanciateObject();
        }


    }
}