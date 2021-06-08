using ReadyGamerOne.Global;
using ReadyGamerOne.Script;
using UnityEngine;

namespace ReadyGamerOne.View.Effects
{
    public class ScollTransition :AbstractTransition
    {
        public ScollTransition(TransionType type, float delay, float time) : base(type,time,delay)
        {
        }

        protected override void OnBegin(AbstractPanel panelBefore,AbstractPanel newPanel)
        {
            var speedX = Screen.width / this.time;
            var speedY = Screen.height / this.time;
            
            newPanel.Enable();

            switch (type)
            {
                case TransionType.PageDown:
                    newPanel.m_TransFrom.position = GlobalVar.GCanvasButton;
                    MainLoop.Instance.UpdateForSeconds(
                        () =>
                        {
                            if (panelBefore != null)
                                panelBefore.m_TransFrom.Translate(Vector3.up * speedY*Time.deltaTime, Space.World);
                            newPanel.m_TransFrom.Translate(Vector3.up * speedY*Time.deltaTime, Space.World);
                        }, 
                        this.time);
                    break;
                case TransionType.PageRight:
                    newPanel.m_TransFrom.position = GlobalVar.GCanvasRight;
                    MainLoop.Instance.UpdateForSeconds(
                        () =>
                        {
                            if (panelBefore != null)
                                panelBefore.m_TransFrom.Translate(Vector3.left * speedX*Time.deltaTime, Space.World);
                            newPanel.m_TransFrom.Translate(Vector3.left * speedX*Time.deltaTime, Space.World);
                        }, 
                        this.time);
                    break;
                case TransionType.PageUp:
                    newPanel.m_TransFrom.position = GlobalVar.GCanvasTop;
                    MainLoop.Instance.UpdateForSeconds(
                        () =>
                        {
                            if (panelBefore != null)
                                panelBefore.m_TransFrom.Translate(Vector3.down * speedY*Time.deltaTime, Space.World);
                            newPanel.m_TransFrom.Translate(Vector3.down * speedY*Time.deltaTime, Space.World);
                        }, 
                        this.time);
                    break;
                case TransionType.PageLeft:
                    newPanel.m_TransFrom.position = GlobalVar.GCanvasLeft;
                    MainLoop.Instance.UpdateForSeconds(
                        () =>
                        {
                            if (panelBefore != null)
                                panelBefore.m_TransFrom.Translate(Vector3.right * speedX*Time.deltaTime, Space.World);
                            newPanel.m_TransFrom.Translate(Vector3.right * speedX*Time.deltaTime, Space.World);
                        }, 
                        this.time);
                    break;
            }
        }

        protected override void OnEnd(AbstractPanel panelBefore,AbstractPanel newPanel)
        {
            newPanel.m_TransFrom.position = GlobalVar.GCanvasObj.transform.position;
            if(panelBefore!=null&&panelBefore.IsVisable())
                panelBefore.Disable();
        }

    }
}

