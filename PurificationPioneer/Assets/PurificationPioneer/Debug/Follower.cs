using UnityEngine;

namespace PurificationPioneer
{
    public class Follower : MonoBehaviour
    {
        public enum FollowType
        {
            DirectFollow,
            
        }
        public Transform m_Target;
        public float m_MaxSpeed = 3;
        public float m_MaxChangeSpeed = 0.1f;
        public FollowType m_FollowType;
        public Vector3 m_LastSpeed=Vector3.down;
        
        private void Update()
        {
            if (m_FollowType == FollowType.DirectFollow)
            {
                
                var expectedSpeed = (m_Target.position - transform.position).normalized * m_MaxSpeed;
                
                var speedChange = expectedSpeed - m_LastSpeed; 
                if (speedChange.magnitude > m_MaxChangeSpeed)
                {
                    speedChange = speedChange.normalized * m_MaxChangeSpeed;
                }
                
                m_LastSpeed += speedChange;
                
                transform.position += m_LastSpeed * Time.deltaTime;
            }
        }
    }
}