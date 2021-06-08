using System;
using System.Text;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Script;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class GameEndPanelScript : MonoBehaviour
    {
        public Text m_TitleText;
        public Text m_ScoreText;
        public Text m_CoinText;
        public Text m_DiamondText;
        public Text m_ExpText;
        public Text m_RankExpText;
        public GameObject m_ExpLevelUpFlag;
        public GameObject m_RankLevelUpFlag;

        public void Init()
        {
            m_TitleText.text = "Victory";
            if (GlobalVar.GameEndTick.score == 5)
                m_ScoreText.text = "S";
            else
            {
                m_ScoreText.text = ((char) ('A' + GlobalVar.GameEndTick.score)).ToString();
            }

            var packageInfo = GlobalVar.GameEndTick.packageInfo;
            m_CoinText.text = $"+{packageInfo.ucoin - GlobalVar.Ucoin}";
            m_DiamondText.text = $"+{packageInfo.udiamond - GlobalVar.Udiamond}";

            var expLevelUp = packageInfo.ulevel != GlobalVar.Ulevel;
            var curExp = packageInfo.uexp;
            if (expLevelUp)
            {
                curExp += 100;
                m_ExpLevelUpFlag.SetActive(true);
            }
            m_ExpText.text = $"+{curExp - GlobalVar.Uexp}";
            
            var rankLevelUp = packageInfo.urank != GlobalVar.Urank;
            var curRankExp = packageInfo.urankExp;
            if (rankLevelUp)
            {
                curRankExp += 125;
                m_RankLevelUpFlag.SetActive(true);
            }
            m_RankExpText.text = $"+{curRankExp - GlobalVar.UrankExp}";
            
            GlobalVar.UpdateUserInfoByGameEndInfo();
        }

        public void BackToHome()
        {
            SceneMgr.LoadSceneWithSimpleLoadingPanel(SceneName.Home);
        }
    }
}