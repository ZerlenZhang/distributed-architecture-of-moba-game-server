using Cinemachine;
using UnityEngine;

namespace PurificationPioneer.Script
{    
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [AddComponentMenu("Cinemachine/MyCinemachineFreeLook")]
    public class MyCinemachineFreeLook:CinemachineFreeLook
    {
        public bool positionFollow = true;

        private Vector3 startPosition;
        private bool inited = false;

        public void Init(Vector3 startPosition)
        {
            inited = true;
            this.startPosition = startPosition;
        }
        protected override void Update()
        {
            if (positionFollow && inited)
            {
                var delta = Follow.position - startPosition;
                startPosition = Follow.position;
                Camera.main.transform.position += delta;
            }
            base.Update();
        }
    }
}