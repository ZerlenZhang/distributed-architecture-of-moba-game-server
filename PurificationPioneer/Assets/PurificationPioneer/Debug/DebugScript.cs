using UnityEngine;

namespace PurificationPioneer.Script
{
    public class DebugScript : MonoBehaviour
    {
        public Transform start;
        public Transform end;
        public LayerMask layerMask;
        public float sphereRadius = 0.5f;
        [ContextMenu("Raycast")]
        private void Raycast()
        {
            var dir = end.position - start.position;
            var distance = dir.magnitude;
            
            foreach (var raycast in Physics.RaycastAll(start.position,dir,distance,layerMask))
            {
                Debug.Log($"[RayCast] {raycast.collider.name}");
            }

            foreach (var raycast in Physics.SphereCastAll(start.position,sphereRadius,dir,distance,layerMask))
            {
                Debug.Log($"[SphereCast] {raycast.collider.name}");
            }
        }
        
        
        [ContextMenu("Show CapsuleCollider Direction")]
        private void ShowCapsuleColliderDirection()
        {
            var col = GetComponent<CapsuleCollider>();
            if (col)
            {
                Debug.Log(col.direction);
            }
        }
    }
}