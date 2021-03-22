using System.Text;
using PurificationPioneer.Script;
using UnityEngine;

namespace PurificationPioneer
{
    public class PhysicsTester : MonoBehaviour
    {
        public bool enablePpPhysicsLog = true;
        public bool enableUnityPhysicsLog = true;
        public bool enableLogStayMsg = false;
        public bool showPhysicFrameId = false;
        public bool showInteractType = false;
        public bool showColliderType = false;

        #region Unity Physics

        private void OnTriggerEnter(Collider other)
        {
            if (!enableUnityPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showColliderType)
                msg.Append($" [Trigger]");
            if (showInteractType)
                msg.Append($" [Enter]");
            msg.Append($" {name} OnTriggerEnter: {other.name}");
            Debug.Log(msg);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enableUnityPhysicsLog)
                return;
            if (!enableLogStayMsg)
                return;
            
            var msg = new StringBuilder();
            if (showColliderType)
                msg.Append($" [Trigger]");
            if (showInteractType)
                msg.Append($" [Stay]");
            msg.Append($" {name} OnTriggerStay: {other.name}");
            Debug.Log(msg);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enableUnityPhysicsLog)
                return;
            
            var msg = new StringBuilder();
            if (showColliderType)
                msg.Append($" [Trigger]");
            if (showInteractType)
                msg.Append($" [Exit]");
            msg.Append($" {name} OnTriggerExit: {other.name}");
            Debug.Log(msg);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!enableUnityPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showColliderType)
                msg.Append($" [Collision]");
            if (showInteractType)
                msg.Append($" [Enter]");
            msg.Append($" {name} OnCollisionEnter: {other.collider.name}, Impulse:{other.impulse}");
            Debug.Log(msg);
        }

        private void OnCollisionStay(Collision other)
        {
            if (!enableUnityPhysicsLog)
                return;
            if (!enableLogStayMsg)
                return;
            var msg = new StringBuilder();
            if (showColliderType)
                msg.Append($" [Collision]");
            if (showInteractType)
                msg.Append($" [Stay]");
            msg.Append($" {name} OnCollisionStay: {other.collider.name}, impulse:{other.impulse}");
            Debug.Log(msg);
        }

        private void OnCollisionExit(Collision other)
        {
            if (!enableUnityPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showColliderType)
                msg.Append($" [Collision]");
            if (showInteractType)
                msg.Append($" [Exit]");
            msg.Append($" {name} OnCollisionExit: {other.collider.name}");
            Debug.Log(msg);
        }        

        #endregion

        #region PpPhysics

        private void PpOnCollisionEnter(PpRaycastHit other)
        {
            if (!enablePpPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showPhysicFrameId)
                msg.Append($"[{PpPhysics.physicsFrameId}]");
            if (showColliderType)
                msg.Append($" [Collision]");
            if (showInteractType)
                msg.Append($" [Enter]");
            msg.Append($" {name} PpOnCollisionEnter: {other.Collider.name}[Normal-{other.Normal}]");
            Debug.Log(msg);
        }

        private void PpOnCollisionExit(PpRaycastHit other)
        {
            if (!enablePpPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showPhysicFrameId)
                msg.Append($"[{PpPhysics.physicsFrameId}]");
            if (showColliderType)
                msg.Append($" [Collision]");
            if (showInteractType)
                msg.Append($" [Exit]");
            msg.Append($" {name} PpOnCollisionExit: {other.Collider.name}");
            Debug.Log(msg);
        }

        private void PpOnCollisionStay(PpRaycastHit other)
        {
            if (!enablePpPhysicsLog)
                return;
            if (!enableLogStayMsg)
                return;
            var msg = new StringBuilder();
            if (showPhysicFrameId)
                msg.Append($"[{PpPhysics.physicsFrameId}]");
            if (showColliderType)
                msg.Append($" [Collision]");
            if (showInteractType)
                msg.Append($" [Stay]");
            msg.Append($" {name} PpOnCollisionStay: {other.Collider.name}");
            Debug.Log(msg);
        }

        private void PpOnTriggerEnter(Collider other)
        {
            if (!enablePpPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showPhysicFrameId)
                msg.Append($"[{PpPhysics.physicsFrameId}]");
            if (showColliderType)
                msg.Append($" [Trigger]");
            if (showInteractType)
                msg.Append($" [Enter]");
            msg.Append($" {name} PpOnTriggerEnter: {other.name}");
            Debug.Log(msg);
        }

        private void PpOnTriggerStay(Collider other)
        {
            if (!enablePpPhysicsLog)
                return;
            if (!enableLogStayMsg)
                return;
            var msg = new StringBuilder();
            if (showPhysicFrameId)
                msg.Append($"[{PpPhysics.physicsFrameId}]");
            if (showColliderType)
                msg.Append($" [Trigger]");
            if (showInteractType)
                msg.Append($" [Stay]");
            msg.Append($" {name} PpOnTriggerStay: {other.name}");
            Debug.Log(msg);
        }

        private void PpOnTriggerExit(Collider other)
        {
            if (!enablePpPhysicsLog)
                return;
            var msg = new StringBuilder();
            if (showPhysicFrameId)
                msg.Append($"[{PpPhysics.physicsFrameId}]");
            if (showColliderType)
                msg.Append($" [Trigger]");
            if (showInteractType)
                msg.Append($" [Enter]");
            msg.Append($" {name} PpOnTriggerExit: {other.name}");
            Debug.Log(msg);
        }        

        #endregion
    }
}