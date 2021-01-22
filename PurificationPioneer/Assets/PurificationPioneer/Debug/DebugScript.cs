using UnityEngine;

namespace PurificationPioneer.Script
{
    public class DebugScript : MonoBehaviour
    {
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