using Es.InkPainter;
using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class PpCollisionPainter : MonoBehaviour
    {
        public BrushConfigAsset brushConfig;
        public bool paintSelf = true;
        public int callDelta = 3;
        private int _adder = 0;
        public float detectLength = 0.001f;
        public void Awake()
        {
            if (paintSelf)
            {
                GetComponent<MeshRenderer>().material.color = brushConfig.brush.Color;
            }
        }

        private void PpOnCollisionStay(PpRaycastHit other)
        {
            var rig = GetComponent<PpRigidbody>();
            if (rig)
            {
                rig.RigidbodyHelper.RaycastNoAlloc(Vector3.down, detectLength,
                    hitInfo =>
                    {
                        var canvas = other.Collider.GetComponent<InkCanvas>();
                        if (canvas != null)
                        {
                            if (_adder == 0)
                            {
                                canvas.Paint(brushConfig.brush, hitInfo);
                            }
            
                            if (++_adder == callDelta)
                            {
                                _adder = 0;
                            }
                        }
                    });
            }
        }
    }
}