using System;
using PurificationPioneer.Global;
using PurificationPioneer.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class RobotBullet : MonoBehaviour, IFrameSyncUnit
    {
        public float m_GravityScale = 1.0f;
        public float m_ShootSpeed = 10;
        public float m_Life = 5f;
        private Vector3 m_LogicPos;
        private Vector3 m_LogicVel;
        private LayerMask m_AttackLayer;
        private int m_Damage;
        private bool m_Initialize;
        private float m_StartFrameId;
        public void Init(Vector3 initPos, Vector3 initVel, LayerMask attackLayer, int damage)
        {
            m_Initialize = true;
            m_LogicPos = initPos;
            m_LogicVel = initVel.normalized * m_ShootSpeed;
            m_AttackLayer = attackLayer;
            m_Damage = damage;

            transform.position = m_LogicPos;
            m_StartFrameId = FrameSyncMgr.FrameId;

            FrameSyncMgr.AddFrameSyncUnit(this);
        }
        
        private void Update()
        {
            if (!m_Initialize)
            {
                return;
            }
            var acceleration = Physics.gravity * m_GravityScale;
            var time = FrameSyncMgr.DelayTime;
            var vel = m_LogicVel + acceleration * time;
            var perfectLogicPos = m_LogicPos + vel * time;

            var currentPos = transform.position;
            transform.position = Vector3.Lerp(currentPos, perfectLogicPos, 0.5f);
            transform.up = vel;
        }

        private void OnDestroy()
        {
            FrameSyncMgr.RemoveFrameSyncUnit(this);
        }        
        
        public int InstanceId => gameObject.GetInstanceID();
        
        public void OnLogicFrameUpdate(float deltaTime)
        {
            var acceleration = Physics.gravity * m_GravityScale;
            var vel = m_LogicVel + acceleration * deltaTime;
            var perfectLogicPos = m_LogicPos + vel * deltaTime;

            m_LogicVel = vel;
            m_LogicPos = perfectLogicPos;
            if ((FrameSyncMgr.FrameId - m_StartFrameId) * GlobalVar.LogicFrameDeltaTime.ToFloat() > m_Life)
            {
                Destroy(gameObject);
            }
        }
    }
}