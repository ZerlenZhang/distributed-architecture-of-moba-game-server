using System;
using PurificationPioneer.Global;
using PurificationPioneer.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class RobotBullet : MonoBehaviour, IFrameSyncUnit
    {
        [HideInInspector] public bool m_WorkAsLocal;
        
        public float m_GravityScale = 1.0f;
        public float m_ShootSpeed = 10;
        public float m_Life = 5f;


        public float m_Hardness;
        public float m_Radius;
        public float m_Stength;
        public Color m_Color = Color.red;
        
        private Vector3 m_LogicPos;
        private Vector3 m_LogicVel;
        private LayerMask m_AttackLayer;
        private int m_Damage;
        private bool m_Initialize;
        private float m_StartFrameId;

        private float m_CreateTime;
        
        public void Init(bool workAsLocal, Vector3 initPos, Vector3 initVel, LayerMask attackLayer, int damage, Color paintColor)
        {
            m_WorkAsLocal = workAsLocal;
            m_Initialize = true;
            m_LogicPos = initPos;
            m_LogicVel = initVel.normalized * m_ShootSpeed;
            m_AttackLayer = attackLayer;
            m_Damage = damage;
            m_Color = paintColor;
            m_CreateTime = Time.timeSinceLevelLoad;

            transform.position = m_LogicPos;
            m_StartFrameId = FrameSyncMgr.FrameId;

            FrameSyncMgr.AddFrameSyncUnit(this);
        }
        public void Init(bool workAsLocal, Vector3 initPos, Vector3 initVel, LayerMask attackLayer, int damage, Material paintMaterial)
        {
            Init(workAsLocal, initPos, initVel, attackLayer, damage, paintMaterial.color);
        }
        
        private void Update()
        {

            var currentPos = transform.position;
            Vector3 acceleration, vel, perfectLogicPos;

            if (m_WorkAsLocal)
            {
                acceleration = Physics.gravity * m_GravityScale;
                vel = m_LogicVel + acceleration * Time.deltaTime;
                perfectLogicPos = m_LogicPos + vel * Time.deltaTime;

                transform.position = Vector3.Lerp(currentPos, perfectLogicPos, 0.5f);
                transform.up = vel;
                
                
                m_LogicVel = vel;
                m_LogicPos = perfectLogicPos;
                if (Time.timeSinceLevelLoad - m_CreateTime > m_Life)
                {
                    Destroy(gameObject);
                }

                return;
            }
            if (!m_Initialize)
            {
                return;
            }
            acceleration = Physics.gravity * m_GravityScale;
            var time = FrameSyncMgr.DelayTime;
            vel = m_LogicVel + acceleration * time;
            perfectLogicPos = m_LogicPos + vel * time;

            transform.position = Vector3.Lerp(currentPos, perfectLogicPos, 0.5f);
            transform.up = vel;
        }
        
        
        private void OnCollisionEnter(Collision other) {
            var p = other.collider.GetComponent<Paintable>();
            if(p != null){
                Vector3 pos = other.contacts[0].point;
                PaintManager.Instance.Paint(p, pos, m_Radius, m_Hardness, m_Stength, m_Color);
                Destroy(gameObject);
            }
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