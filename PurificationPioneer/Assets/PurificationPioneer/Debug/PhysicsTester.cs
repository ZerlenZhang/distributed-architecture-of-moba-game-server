using UnityEngine;

namespace PurificationPioneer
{
    public class PhysicsTester : MonoBehaviour
    {
        private void PpOnCollisionEnter(Collider other)
        {
            Debug.Log($"[Physics] {name} collision enter: {other.name}");
        }

        private void PpOnCollisionExit(Collider other)
        {
            Debug.Log($"[Physics] {name} collision exit: {other.name}");
        }

        private void PpOnCollisionStay(Collider other)
        {
            Debug.Log($"[Physics] {name} collision stay: {other.name}");
        }

        private void PpOnTriggerEnter(Collider other)
        {
            Debug.Log($"[Physics] {name} trigger enter: {other.name}");
        }

        private void PpOnTriggerStay(Collider other)
        {
            Debug.Log($"[Physics] {name} trigger stay: {other.name}");
        }

        private void PpOnTriggerExit(Collider other)
        {
            Debug.Log($"[Physics] {name} trigger exit: {other.name}");
        }
    }
}