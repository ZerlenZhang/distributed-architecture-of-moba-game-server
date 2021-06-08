using System;
using ReadyGamerOne.Script;

namespace ReadyGamerOne.View.Effects
{
    public abstract class AbstractTransition
    {
        protected TransionType type;
        public float time;
        private float delay;
        public event Action<AbstractPanel, AbstractPanel> onBegin;
        public event Action<AbstractPanel, AbstractPanel> onEnd;

        protected AbstractTransition(TransionType type,float time,float delay)
        {
            this.delay = delay;
            this.time = time;
            this.type = type;
        }

        public void PushPanel(AbstractPanel newPanel)
        {
            var panelBefore = PanelMgr.CurrentPanel as AbstractPanel;
            MainLoop.Instance.ExecuteLater(() =>
            {
                OnBegin(panelBefore, newPanel);
                if (onBegin != null)
                    onBegin(panelBefore, newPanel);
                MainLoop.Instance.ExecuteLater(() =>
                {
                    OnEnd(panelBefore, newPanel);
                    if (onEnd != null)
                        onEnd(panelBefore, newPanel);
                }, this.time);
            }, this.delay);
        }

        protected abstract void OnBegin(AbstractPanel panelBefore,AbstractPanel newPanel);
        protected abstract void OnEnd(AbstractPanel panelBefore,AbstractPanel newPanel);
    }
}

