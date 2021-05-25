using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class RobotController:PpCharacterController
    {
        [SerializeField] private Transform m_BulletGenPos;
        [SerializeField] private float m_AttackDeltaTime = 0.5f;
        [SerializeField] private GameObject m_BulletPrefab;
        [SerializeField] private LayerMask m_AttackLayer;
        [SerializeField] private int m_Attack;
        
        
        private float m_LastTime = 0;


        protected override void InitCharacter(bool isLocal)
        {
            base.InitCharacter(isLocal);
            m_LastTime = Time.timeSinceLevelLoad;
        }

        protected override void OnCommonAttack(int faceX, int faceY, int faceZ)
        {
            base.OnCommonAttack(faceX, faceY, faceZ);
            var current = Time.timeSinceLevelLoad;
            if (current - m_LastTime > m_AttackDeltaTime)
            {
                // attack
                var bullet = Instantiate(m_BulletPrefab);
                var robotBullet = bullet.GetComponent<RobotBullet>();

                var forward = m_BulletGenPos.forward;
                var dir = new Vector3(forward.x, faceY.ToFloat(), forward.z);

                if (m_WorkAsLocal)
                {
                    robotBullet.Init(true, m_BulletGenPos.position, dir, m_AttackLayer, m_Attack, 
                        Color.red);
                }
                else
                {
                    robotBullet.Init(false, m_BulletGenPos.position, dir, m_AttackLayer, m_Attack, 
                        GlobalVar.LocalSeatId % 2==0
                            ? GameSettings.Instance.LeftMaterial
                            : GameSettings.Instance.RightMaterial);                    
                }

                m_LastTime = current;
            }
        }
    }
}