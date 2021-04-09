using DialogSystem.Model;
using ReadyGamerOne.Common;
using ReadyGamerOne.View;
using UnityEngine.EventSystems;

namespace DialogSystem.View
{
    public class DialogUnitUI:AbstractUI
    {
        protected DialogUnitInfo m_dialogUnitInfo;
        public DialogUnitUI(DialogUnitInfo info)
        {
            m_dialogUnitInfo = info;
        }

        public override void DestroyThis(PointerEventData eventData = null)
        {
            base.DestroyThis(eventData);
            CEventCenter.BroadMessage(Scripts.DialogSystem.EndThisDialogUnit,m_dialogUnitInfo.abstractAbstractDialogInfoAsset.name);
//            Debug.Log("DialogUnitUI_DestoryThis_CEventCenter_BroadMessage:"+m_dialogUnitInfo.abstractAbstractDialogInfoAsset.name);
        }
        
        protected bool canGoNext = false;
        private void SetCanGoNext() => this.canGoNext = true;

        protected override void OnAddListener()
        {
            base.OnAddListener();
            CEventCenter.AddListener(Scripts.DialogSystem.CanGoNext, SetCanGoNext);
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            CEventCenter.RemoveListener(Scripts.DialogSystem.CanGoNext, SetCanGoNext);
        }
    }
}