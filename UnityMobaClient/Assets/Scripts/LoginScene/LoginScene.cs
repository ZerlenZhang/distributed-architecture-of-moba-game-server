using System;
using Moba.Const;
using Moba.Protocol;
using ReadyGamerOne.Common;
using ReadyGamerOne.View;
using UnityEngine;

namespace Moba.Script
{
    public class LoginScene:SceneHelper
    {
        protected override void Start()
        {
            base.Start();
            CEventCenter.AddListener(Message.LoginLogicServerSuccess,OnLoginLogicServerSuccess);
            CEventCenter.AddListener(Message.GetUgameInfoSuccess,OnGetUgaemInfoSuccess);
            CEventCenter.AddListener(Message.GameStart, OnGameStart);
        }


        private void OnDestroy()
        {
            CEventCenter.RemoveListener(Message.LoginLogicServerSuccess,OnLoginLogicServerSuccess);
            CEventCenter.RemoveListener(Message.GetUgameInfoSuccess,OnGetUgaemInfoSuccess);
            CEventCenter.RemoveListener(Message.GameStart, OnGameStart);
        }

        private void OnGameStart()
        {
            Debug.Log("GameStart");
            PanelMgr.PushPanelWithMessage(PanelName.LoadingPanel,Message.LoadSceneAsync,SceneName.Battle);
        }

        private void OnLoginLogicServerSuccess()
        {
            CEventCenter.RemoveListener(Message.LoginLogicServerSuccess,OnLoginLogicServerSuccess);
            Debug.Log("登陆逻辑服务器成功");
            PanelMgr.PushPanel(PanelName.HomePanel);
        }

        private void OnGetUgaemInfoSuccess()
        {
            CEventCenter.RemoveListener(Message.GetUgameInfoSuccess, OnGetUgaemInfoSuccess);
            Debug.Log("获取Moba信息成功，开始正在登陆逻辑服务器");
            LogicServiceProxy.Instance.LoginLogicServer();
        }
    }
}