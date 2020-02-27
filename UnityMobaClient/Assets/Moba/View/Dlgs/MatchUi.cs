using gprotocol;
using Moba.Const;
using Moba.Global;
using Moba.Protocol;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Moba.View.Dlgs
{
    public class MatchUi : MonoBehaviour
    {
        public ScrollRect scrollRect;
        
        private void Start()
        {
            CEventCenter.AddListener<int>(Message.PlayerExitRoom, OnPlayerExitRoom);
            CEventCenter.AddListener(Message.LeaveRoomSuccess,OnLeaveSuccess);
            CEventCenter.AddListener<PlayerEnterRoom>(Message.PlayerEnterRoom,OnPlayerEnterRoom);
            CEventCenter.AddListener(Message.GameStart, OnGameStart);
        }

        private void OnGameStart()
        {
            Destroy(this.gameObject);
        }

        private void OnPlayerExitRoom(int obj)
        {
            GameObject.Destroy(this.scrollRect.content.GetChild(obj).gameObject);
            this.scrollRect.content.sizeDelta=new Vector2(0,
                this.scrollRect.content.childCount*91);
        }

        private void OnLeaveSuccess()
        {
            Destroy(this.gameObject);
        }

        private void OnDestroy()
        {           
            CEventCenter.RemoveListener(Message.GameStart, OnGameStart);
            CEventCenter.RemoveListener<int>(Message.PlayerExitRoom, OnPlayerExitRoom);
            CEventCenter.RemoveListener(Message.LeaveRoomSuccess,OnLeaveSuccess);
            CEventCenter.RemoveListener<PlayerEnterRoom>(Message.PlayerEnterRoom,OnPlayerEnterRoom);
        }

        private void OnPlayerEnterRoom(PlayerEnterRoom msg)
        {
            var obj = ResourceMgr.InstantiateGameObject(UiName.PlayerInfo, scrollRect.content);
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var nickText = obj.transform.Find("NickText").GetComponent<Text>();
            var levelText = obj.transform.Find("LevelText").GetComponent<Text>();

            icon.sprite = ResourceMgr.GetAsset<Sprite>("Avator_" + msg.uface);
            nickText.text = msg.unick;
            levelText.text = "Lv " + msg.usex;
            
            this.scrollRect.content.sizeDelta=new Vector2(0,
                this.scrollRect.content.childCount*91);
            
        }

        public void OnClose()
        {
            
        }

        public void OnStartMatch()
        {
            LogicServiceProxy.Instance.EnterZone(NetInfo.zoneId);
        }

        public void OnStopMatch()
        {
            LogicServiceProxy.Instance.ExitRoom();
        }
    }
}