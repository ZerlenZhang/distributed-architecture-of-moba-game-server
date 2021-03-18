using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace PurificationPioneer.AI
{
    public class MoveToPosition: Action
    {
        public SharedVector3 in_TargetPosition;
        public SharedFloat in_MoveSpeed;
        private BasicAgentController _agentController;
    
        public override void OnAwake()
        {
            base.OnAwake();
            _agentController = GetComponent<BasicAgentController>();
        }
    
        public override void OnStart()
        {
            base.OnStart();
            // Debug.Log($"[AI][{PpPhysics.physicsFrameId}-{FrameSyncMgr.FrameId}][{_agentController.name}] Start");
            _agentController.WalkTowards(in_TargetPosition.Value,in_MoveSpeed.Value);
        }
    
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Running;
        }
    }

}