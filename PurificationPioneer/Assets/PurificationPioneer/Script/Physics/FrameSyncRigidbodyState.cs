using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{

    /// <summary>
    /// 用于帧同步的刚体状态类，可以使用刚体状态让刚体回溯或模拟运行一段时间
    /// </summary>
    public class FrameSyncRigidbodyState
    {
        protected readonly PpRigidbody _rigidbody;
        private Vector3 _logicPosition;
        private IPpRigidbodyState _rigidbodyState;
        
        private IPpRigidbodyState RigidBodyState
        {
            get
            {
                if (null == _rigidbodyState)
                {
                    _rigidbodyState = _rigidbody.GetState();
                }

                return _rigidbodyState;
            }
        }

        #region 状态的保存和恢复

        /// <summary>
        /// 保存当前状态，
        /// </summary>
        public void SaveState()
        {
            LogicPosition = _rigidbody.Position;
            _rigidbody.GetStateNoAlloc(RigidBodyState);
        }

        /// <summary>
        /// 应用上次保存的状态并模拟一段时间
        /// </summary>
        /// <param name="deltaTime"></param>
        public void ApplyAndSimulate(float deltaTime)
        {
            _rigidbody.ApplyRigidbodyState(RigidBodyState);
            _rigidbody.Simulate(deltaTime);
        }        

        #endregion

        public FrameSyncRigidbodyState(PpRigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            Assert.IsTrue(_rigidbody);
            LogicPosition = _rigidbody.Position;
        }
        public FrameSyncRigidbodyState(PpRigidbody rigidbody, Vector3 initPosition)
        {
            _rigidbody = rigidbody;
            Assert.IsTrue(_rigidbody);
            LogicPosition = initPosition;
            RendererPosition = initPosition;
        }

        public Vector3 LogicPosition
        {
            get => _logicPosition;
            private set => _logicPosition = value;
        }

        public Vector3 RendererPosition
        {
            get => _rigidbody.Position;
            set => _rigidbody.Position = value;
        }
        
        public Vector3 Velocity
        {
            set => _rigidbody.Velocity = value;
        }
        public Vector3 Direction
        {
            get => _rigidbody.transform.forward;
            set => _rigidbody.transform.forward = value;
        }
    }
}