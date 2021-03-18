using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.AI
{
    public class BasicAgentController:MonoBehaviour,IFrameSyncUnit
    {
        #region private fields

        [SerializeField] private BehaviorTree _behaviorTree;
        [SerializeField] private PpRigidbody _rigidbody;
        
        
        //move
        private Vector3? _targetPosition=null;
        //destroy
        private bool _isDestroyed;

        #endregion
        protected virtual void Start()
        {
            FrameSyncMgr.AddFrameSyncUnit(this);
        }

        
        #region IFrameSyncUnit

        public int InstanceId => GetInstanceID();
        public void OnLogicFrameUpdate(float deltaTime)
        {
            if (_targetPosition.HasValue)
            {
                var distance = Vector3.Distance(_rigidbody.Position, _targetPosition.Value);
                
                // Debug.Log($"距离：{distance}, 检测距离：{GameSettings.Instance.MinDetectableDistance}");
                
                if (distance <= 1)
                {
                    Debug.Log($"[AI][Arrive][{PpPhysics.physicsFrameId}-{FrameSyncMgr.FrameId}]到达");
                    _targetPosition = null;
                    _rigidbody.Velocity = new Vector3(
                        0, _rigidbody.Velocity.y, 0);
                    _behaviorTree.DisableBehavior();                    
                }
            }

            if (_behaviorTree.ExecutionStatus != TaskStatus.Running)
            {
                Debug.Log($"[AI][Invoke][{PpPhysics.physicsFrameId}-{FrameSyncMgr.FrameId}][{_rigidbody.name}][Velocity-{_rigidbody.Velocity}][Pos-{_rigidbody.Position}]");
                //这里会有延迟，不会瞬间触发
                _behaviorTree.EnableBehavior();
            }
            
            // Debug.Log($"[AI][FrameSync][{PpPhysics.physicsFrameId}-{FrameSyncMgr.FrameId}][{_rigidbody.name}][Velocity-{_rigidbody.Velocity}][Pos-{_rigidbody.Position}]");
        }

        #endregion
        
        #region DestroyAgent

        protected void DestroyAgent()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            OnAgentDestroy();
        }

        protected virtual void OnAgentDestroy(bool destroyBySceneLoad=false)
        {;
            FrameSyncMgr.RemoveFrameSyncUnit(this);
        }
        
        private void OnDestroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            OnAgentDestroy(true);
        }        

        #endregion

        #region Move

        /// <summary>
        /// 向某个位置前进
        /// </summary>
        /// <param name="position"></param>
        /// <param name="speed"></param>
        /// <returns>是否已经到达某个位置</returns>
        public bool WalkTowards(Vector3 position, float speed)
        {
            var distance = Vector3.Distance(position, _rigidbody.Position);
            if (distance <= GameSettings.Instance.MinDetectableDistance)
                return true;
            _targetPosition = position;

            var vel = (position - _rigidbody.Position).normalized * speed;
            var expectedVelocity = new Vector3(
                vel.x,
                _rigidbody.Velocity.y,
                vel.z);
            _rigidbody.InternalVelocity=expectedVelocity;
#if DebugMode
            if (GameSettings.Instance.EnableAiLog)
            {
                Debug.Log($"[AI][SetVelocity][{PpPhysics.physicsFrameId}-{FrameSyncMgr.FrameId}][Velocity-{expectedVelocity}");
            }
#endif
            return false;
        }

        #endregion
    }
}