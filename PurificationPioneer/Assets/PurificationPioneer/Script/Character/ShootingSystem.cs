using System;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using PurificationPioneer.Global;
using PurificationPioneer.Paint;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class ShootingSystem : MonoBehaviour
    {
        [SerializeField] private int MaxParticleCount = 75;
        [HideInInspector] public bool m_WorkAsLocal = true;
        [SerializeField] ParticleSystem inkParticle;
        [SerializeField] Transform parentController;
        [SerializeField] Transform splatGunNozzle;
        [SerializeField] CinemachineFreeLook m_FreeLookCamera;

        [SerializeField] private ParticleSystem[] particleSystems;
        
        CinemachineImpulseSource impulseSource;

        private bool m_Init = false;
        private Func<bool> m_IfAttack;
        private bool m_LastAttackState = false;
        private int startFrameId;

        public void Initialize(int shooterSeatId, float paintEfficiencyScale, Func<bool> ifAttack,bool workAsLocal=false, Material paintMaterial = null)
        {
            paintMaterial = paintMaterial ? paintMaterial : GameSettings.Instance.GetMaterialBySeatId(shooterSeatId);
            
            m_Init = true;
            m_IfAttack = ifAttack;
            m_WorkAsLocal = workAsLocal;

            foreach (var particlesController in GetComponentsInChildren<ParticlesController>())
            {
                particlesController.workAsLocal = workAsLocal;
                particlesController.m_SeatId = shooterSeatId;
                particlesController.m_PaintScore = paintEfficiencyScale;
                if (paintMaterial)
                {
                    particlesController.paintColor = paintMaterial.color;
                }
            }

            Assert.IsNotNull(m_IfAttack);
        }
        
        public void Initialize(int shooterSeatId, float paintEfficiencyScale, Func<bool> ifAttack,CinemachineFreeLook camera, Material paintMaterial = null)
        {
            paintMaterial = paintMaterial ? paintMaterial : GameSettings.Instance.GetMaterialBySeatId(shooterSeatId);
            
            Initialize(shooterSeatId, paintEfficiencyScale, ifAttack, true, paintMaterial);
            m_FreeLookCamera = camera;
            impulseSource = m_FreeLookCamera.GetComponent<CinemachineImpulseSource>();
            Assert.IsTrue(m_FreeLookCamera && impulseSource && paintMaterial);
            if (particleSystems != null)
            {
                foreach (var system in particleSystems)
                {
                    var psr = system.GetComponent<ParticleSystemRenderer>();
                    if (psr.enabled)
                    {
                        psr.material = paintMaterial;
                        psr.trailMaterial = paintMaterial;
                    }
                }
            }
        }

        void Update()
        {
            if (!m_Init)
                return;
            
            Vector3 angle = parentController.localEulerAngles;

            var currentAttackState = m_IfAttack();
            if (currentAttackState)
            {
                VisualPolish();
            }

            if (!m_LastAttackState && currentAttackState)
            {
                startFrameId = FrameSyncMgr.FrameId;
                inkParticle.Play();
            }
            else if (m_LastAttackState && !currentAttackState)
            {
                inkParticle.Stop();
            }

            m_LastAttackState = currentAttackState;


            if (m_FreeLookCamera)
            {
                parentController.localEulerAngles
                    = new Vector3(
                        Mathf.LerpAngle(
                            parentController.localEulerAngles.x, 
                            currentAttackState 
                                ? RemapCamera(m_FreeLookCamera.m_YAxis.Value, 0, 1, -25, 25) 
                                : 0, 
                            .3f), 
                        angle.y,
                        angle.z);            
            }

        }

        void VisualPolish()
        {
            if (!DOTween.IsTweening(parentController))
            {
                parentController.DOComplete();
                Vector3 forward = -parentController.forward;
                Vector3 localPos = parentController.localPosition;
                parentController.DOLocalMove(localPos - new Vector3(0, 0, .2f), .03f)
                    .OnComplete(() => parentController.DOLocalMove(localPos, .1f).SetEase(Ease.OutSine));

               impulseSource?.GenerateImpulse();
            }

            if (!DOTween.IsTweening(splatGunNozzle))
            {
                splatGunNozzle.DOComplete();
                splatGunNozzle.DOPunchScale(new Vector3(0, 1, 1) / 1.5f, .15f, 10, 1);
            }
        }

        float RemapCamera(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        private bool saved = false;
        private ParticleSystem.PlaybackState m_PlaybackState;
        private ParticleSystem.Trails m_Trails;
        private ParticleSystem.Particle[] m_Particles;

        private ParticleSystem.Particle[] particles
        {
            get
            {
                if (null == m_Particles)
                {
                    m_Particles = new ParticleSystem.Particle[MaxParticleCount];
                }

                return m_Particles;
            }
        }

        private int m_ValidParticleCount;
        public void SaveState()
        {
            saved = true;
            if (!m_LastAttackState)
                return;
            m_PlaybackState = inkParticle.GetPlaybackState();
            m_Trails = inkParticle.GetTrails();
            m_ValidParticleCount = inkParticle.GetParticles(particles, MaxParticleCount);
        }

        public void ApplyAndSimulate()
        {
            if (!saved)
                return;
            saved = false;
            if (!m_LastAttackState)
                return;
            try
            {
                inkParticle.SetTrails(m_Trails);
            }
            catch
            {
                // ignored
            }

            inkParticle.SetPlaybackState(m_PlaybackState);
            inkParticle.SetParticles(particles,m_ValidParticleCount);
            inkParticle.Simulate((FrameSyncMgr.FrameId-startFrameId)*GlobalVar.LogicFrameDeltaTime.ToFloat());
            inkParticle.Play(true);
        }
    }
}
