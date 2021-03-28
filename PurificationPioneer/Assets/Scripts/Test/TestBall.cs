using PurificationPioneer.Scriptable;
using UnityEngine;

namespace UnityTemplateProjects.Test
{
    public class TestBall : MonoBehaviour
    {
        public BrushConfigAsset m_BrushConfig;

        private void OnCollisionEnter(Collision other)
        {
            var paintable = other.gameObject.GetComponent<Paintable>();
            if (!paintable)
                return;
            Debug.Log($"染色：{other.contacts.Length}");
            foreach (var point in other.contacts)
            {
                PaintManager.Instance.Paint(paintable, point.point, m_BrushConfig);
            }
        }
    }
}