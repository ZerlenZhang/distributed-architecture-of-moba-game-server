using Moba.Global;
using Moba.Protocol;
using UnityEngine;

namespace Moba.View.Dlgs
{
    public class MatchUi : MonoBehaviour
    {
        public void OnClose()
        {
            
        }

        public void OnStartMatch()
        {
            LogicServiceProxy.Instance.EnterZone(NetInfo.zoneId);
        }

        public void OnStopMatch()
        {
            
        }
    }
}