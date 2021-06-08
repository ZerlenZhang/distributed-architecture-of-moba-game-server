using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class HomeSceneMgr : PpSceneMgr<HomeSceneMgr>
    {
        public Transform m_GenPoint;


        private GameObject m_CurrentHero;
        protected override void Start()
        {
            base.Start();
            PanelMgr.PushPanel(PanelName.HomePanel);
        }


        public RenderTexture UpdateHeroPreview(HeroConfigAsset heroConfigAsset)
        {
            if (m_CurrentHero)
            {
                Destroy(m_CurrentHero);
            }

            m_CurrentHero = Instantiate(heroConfigAsset.PreviewPrefab);
            m_CurrentHero.transform.position = m_GenPoint.position;
            
            
            
            
            var mainc = Camera.main;
            var rt = mainc.targetTexture;
            if (rt == null)
            {
                rt = RenderTexture.GetTemporary(1024, 1024);
                mainc.targetTexture = rt;
            }


            return rt;
        }
    }
}