using System;
using UnityEngine;

namespace UnityTemplateProjects.Test
{
    public class TestParticleSystemSimulate : MonoBehaviour
    {
        public ParticleSystem m_Simulate_0_1f;
        public ParticleSystem m_Simulate_0_5f;
        private void Start()
        {
            Debug.Log("Before");
            m_Simulate_0_1f.Simulate(0.1f);
            m_Simulate_0_5f.Simulate(0.5f);
            Debug.Log("After");
        }

        [ContextMenu("go on")]
        private void GoOn()
        {
            m_Simulate_0_1f.Play(true);
            m_Simulate_0_5f.Play(true);
        }
    }
}