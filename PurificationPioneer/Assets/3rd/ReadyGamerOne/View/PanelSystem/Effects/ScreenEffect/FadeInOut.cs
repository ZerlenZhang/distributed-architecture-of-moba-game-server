using System;
using ReadyGamerOne.Global;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.View.Effects
{
    public class FadeInOut : AbstractScreenEffect
    {
        public override void OnBegin(AbstractPanel panelBefore, AbstractPanel newPanel)
        {
            if (panelBefore != null)
                this.time *= 0.5f;
            PanelFadeOut(panelBefore,this.time,()=>PanelFadeIn(newPanel,this.time));



        }
        
        

        public override void OnEnd(AbstractPanel panelbefore, AbstractPanel newPanel)
        {
        }

        public FadeInOut(float time) : base(time)
        {
        }


        private void PanelFadeOut(AbstractPanel panelBefore, float time,Action endCall)
        {
            CanvasGroup cg;
            if (panelBefore != null)
            {
                cg = panelBefore.m_TransFrom.GetComponent<CanvasGroup>();
                if ( cg== null)
                    cg=panelBefore.m_TransFrom.gameObject.AddComponent<CanvasGroup>();
                MainLoop.Instance.UpdateForSeconds(() => { cg.alpha -= Time.deltaTime / time; }, time,
                    () =>
                    {
                        cg.alpha = 1;
                        if(panelBefore.IsVisable())
                            panelBefore.Disable();
                        endCall();
                    });
            }
        }

        private void PanelFadeIn(AbstractPanel newPanel, float time)
        {
            Assert.IsTrue(newPanel!=null);

            if(newPanel.IsVisable()==false)
                newPanel.Enable();
            
            CanvasGroup cg;
            cg = newPanel.m_TransFrom.GetComponent<CanvasGroup>();
            
            if ( cg== null)
                cg=newPanel.m_TransFrom.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            MainLoop.Instance.UpdateForSeconds(() =>
                {
                    if(!newPanel.IsVisable())
                        return;
                    cg.alpha += Time.deltaTime / time;
                }, 
                time,
                () =>
                {
                    if(!newPanel.IsVisable())
                        return;
                    newPanel.m_TransFrom.position = GlobalVar.GCanvasObj.transform.position;
                });



        }
        
    }
}

