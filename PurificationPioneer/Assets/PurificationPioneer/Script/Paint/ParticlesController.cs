using System.Collections.Generic;
using PurificationPioneer.Const;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Paint
{
    public class ParticlesController: MonoBehaviour
    {
        [HideInInspector] public bool workAsLocal=true;
        [HideInInspector] public int m_SeatId;
        [HideInInspector] public float m_PaintScore;
        public Color paintColor;
        
        public float minRadius = 0.05f;
        public float maxRadius = 0.2f;
        public float strength = 1;
        public float hardness = 1;
        [Space]
        ParticleSystem part;
    
        ParticleSystem Part
        {
            get
            {
                if (!part)
                {
                    part = GetComponent<ParticleSystem>();
                    
                    Assert.IsTrue(part);
                }
    
                return part;
            }
        }
        
        List<ParticleCollisionEvent> collisionEvents= new List<ParticleCollisionEvent>();
        
    
        void OnParticleCollision(GameObject other)
        {
            if (!workAsLocal && !FrameSyncMgr.IsSimulating)
                return;
            
            int numCollisionEvents = Part.GetCollisionEvents(other, collisionEvents);
    
            Paintable p = other.GetComponent<Paintable>();
            if(p != null){
                for  (int i = 0; i< numCollisionEvents; i++){
                    Vector3 pos = collisionEvents[i].intersection;
                    float radius = Random.Range(minRadius, maxRadius);
                    PaintManager.Instance.Paint(p, pos, radius, hardness, strength, paintColor);
    
    
                    GameSettings.Instance.BroadScore(m_SeatId, m_PaintScore);
                }
            }
        }
    }
}
