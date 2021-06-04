using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class WarInfoAreaUi : MonoBehaviour
    {
        public Text m_TimeText;
        public Image m_LeftImage;
        public Text m_LeftScoreText;
        public Text m_RightScoreText;

        public Text m_LeftCountText;
        public Text m_RightCountText;

        private float m_LeftScore;
        private float m_RightScore;
        
        private void OnEnable()
        {
            OnChangeScore(0, 0);
            CEventCenter.AddListener<int, float>(Message.ChangeScore, OnChangeScore);
            CEventCenter.AddListener<int>(Message.OnTimeLosing, OnTimeLosing);
        }

        private void OnDisable()
        {        
            CEventCenter.RemoveListener<int, float>(Message.ChangeScore, OnChangeScore);
            CEventCenter.RemoveListener<int>(Message.OnTimeLosing, OnTimeLosing);
        }
        
        private void OnChangeScore(int seatId, float score)
        {
            float leftChange=0, rightChange=0;
            if (GameSettings.Instance.IsSeatIdLeft(seatId))
            {
                leftChange = score;
            }
            else
            {
                rightChange = score;
            }
            
            
            m_LeftScore += leftChange;
            m_RightScore += rightChange;

            var all = m_LeftScore + m_RightScore;

            m_LeftImage.fillAmount = Mathf.Abs(all) <= Mathf.Epsilon
                ? 0.5f
                : m_LeftScore / all;

            var leftInt = (int) (m_LeftImage.fillAmount * 100);
            m_LeftScoreText.text = $"{leftInt}%";
            m_RightScoreText.text = $"{100 - leftInt}%";
            
            m_LeftCountText.text = $"【{m_LeftScore}】";
            m_RightCountText.text = $"【{m_RightScore}】";
        }
        
        private void OnTimeLosing(int leftSeconds)
        {
            m_TimeText.text = $"{leftSeconds / 60}:{leftSeconds % 60}";
        }
    }
}