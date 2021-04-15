using System;
using UnityEngine;

namespace DialogSystem.Scripts
{
    public class InteractableFlag : MonoBehaviour
    {
        private Transform mainCamera;
        public GameObject characterGameObject;
        public void SetInteractable(bool value)
        {
            gameObject.SetActive(value);
        }

        private void Start()
        {
            mainCamera = Camera.main?.transform;
        }

        private void Update()
        {
            if (!mainCamera)
                return;

            var pos = new Vector3(mainCamera.position.x, transform.position.y, mainCamera.position.z);
            transform.LookAt(pos);
        }
    }
}