using System.Linq;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class CleanerController : PpCharacterController
    {
        [SerializeField] private Transform m_Brush;
        [SerializeField] private float m_Hardness=1;
        [SerializeField] private float m_Radius=1;
        [SerializeField] private float m_Stength=0.5f;
        private float m_AttackDeltaTime = 0.2f;

        private float m_LastTime = 0;

        protected override void InitCharacter(bool isLocal)
        {
            base.InitCharacter(isLocal);
            m_AttackDeltaTime = 1f / HeroConfig.attackSpeed;
        }

        protected override void OnCommonAttack(int faceX, int faceY, int faceZ)
        {
            base.OnCommonAttack(faceX, faceY, faceZ);
            
            var current = Time.timeSinceLevelLoad;
            if (current - m_LastTime > m_AttackDeltaTime)
            {
                // attack
                var ans = Physics.RaycastAll(m_Brush.position, Vector3.down, 2, gameObject.GetUnityDetectLayer());

                if (ans.Any())
                {
                    var first = ans.First();
                    var p = first.collider.GetComponent<Paintable>();
                    if(p){
                        Vector3 pos = first.point;
                        PaintManager.Instance.Paint(p, pos, m_Radius, m_Hardness, m_Stength, GameSettings.Instance.GetMaterialBySeatId(SeatId).color);
                
                        GameSettings.Instance.BroadScore(SeatId, PaintEfficiencyScale);
                    }
                }

                m_LastTime = current;
            }
        }
    }
}