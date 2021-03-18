using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using PurificationPioneer.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using Random = UnityEngine.Random;

namespace PurificationPioneer.AI
{
    public class GetRandomPatrolPointInCircle:Action
    {
        public SharedFloat in_DetectAngle=6f;
        public SharedFloat in_DetectRadius=8f;
        public SharedVector3 out_TargetPosition;

        private PpRigidbody _rigidbody;

        private bool success = false;

        public void OnThisGizmos()
        {
            Gizmos.DrawWireSphere(transform.position,in_DetectRadius.Value);
            Gizmos.color= success ? Color.green : Color.red;
            Gizmos.DrawSphere(out_TargetPosition.Value, 0.5f);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            _rigidbody = GetComponent<PpRigidbody>();
            PurificationPioneerMgr.onDrawGizomos += OnThisGizmos;
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 targetPoint;
            
            while (!GetSuitablePoint(out targetPoint)) ;

            out_TargetPosition.Value = targetPoint;
            Debug.Log($"设定位置：{out_TargetPosition.Value}");
            success = true;
            
            return TaskStatus.Success;
        }

        private bool GetSuitablePoint(out Vector3 point)
        {
            
            var randomAngle = Random.Range(-in_DetectAngle.Value, in_DetectAngle.Value);
            var dir = _rigidbody.transform.forward.RotateDegree(randomAngle, AxisIgnore.Y);
            var distance = Random.Range(0, in_DetectRadius.Value);

            var hit = false;
            _rigidbody.RigidbodyHelper.RaycastNoAlloc(dir,distance,
                hitInfo=>hit=true);

            if (hit)
            {
                point=Vector3.zero;
                return false;
            }

            point = _rigidbody.Position + dir.normalized * distance;
            return true;
        }
    }
}