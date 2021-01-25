using PurificationPioneer.Script;
using UnityEngine;

namespace PurificationPioneer
{
    public class PhysicsTester : MonoBehaviour
    {
        private void PpOnCollisionEnter(PpCollision other)
        {
            Debug.Log($"[Physics] {name} collision enter: {other.Collider.name}");
        }

        private void PpOnCollisionExit(PpCollision other)
        {
            Debug.Log($"[Physics] {name} collision exit: {other.Collider.name}");
        }

        private void PpOnCollisionStay(PpCollision other)
        {
            Debug.Log($"[Physics] {name} collision stay: {other.Collider.name}");
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