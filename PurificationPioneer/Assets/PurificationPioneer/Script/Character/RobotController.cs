using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class RobotController:PpCharacterController
    {
        [SerializeField] private Transform m_BulletGenPos;
        [SerializeField] private GameObject m_BulletPrefab;
        [SerializeField] private LayerMask m_AttackLayer;
        [SerializeField] private int m_Attack;

        [Range(0, 1f)] [SerializeField] private float m_BiggerBulletRate = 0.3f;
        [SerializeField] private float m_BiggerBulletScale = 2.5f;
        
        
        private float m_LastTime = 0;
        private float m_AttackDeltaTime = 0.5f;


        protected override void InitCharacter(bool isLocal)
        {
            base.InitCharacter(isLocal);
            m_LastTime = Time.timeSinceLevelLoad;
            m_AttackDeltaTime = 1f / HeroConfig.attackSpeed;
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

                var bigger = Random.Range(0, 1f) <= m_BiggerBulletRate;
                var radiusScale = (bigger ? m_BiggerBulletScale : 1) * PaintEfficiencyScale;

                if (m_WorkAsLocal)
                {  
                    robotBullet.Init(true, SeatId, radiusScale, m_BulletGenPos.position, dir, m_AttackLayer, m_Attack, 
                        Color.red);
                }
                else
                {
                    robotBullet.Init(false, SeatId, radiusScale, m_BulletGenPos.position, dir, m_AttackLayer, m_Attack,
                        GameSettings.Instance.GetMaterialBySeatId(SeatId));                    
                }

                if (bigger)
                {
                    robotBullet.transform.localScale = Vector3.one * m_BiggerBulletScale;
                }

                m_LastTime = current;
            }
        }
    }
}