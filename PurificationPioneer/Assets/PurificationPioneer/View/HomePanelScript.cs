using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Script;
using PurificationPioneer.Utility;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class HomePanelScript:MonoBehaviour
    {
        public UserInfoRect userInfoRect;
        public MatchUi matchUi;
        public ItemBar coinBarInfo;
        public ItemBar diamondBarInfo;
        public HomeOptionUi homeOptionUi;
        public Button playBtn;
        public Button settingBtn;
        public Button friendBtn;
        public Button mailBtn;
        public Button noticeBtn;
        public Button chatBtn;
        public Button shopBtn;
        public GameObject roomTypeUi;

        public void Init()
        {
            homeOptionUi.Init();
        }
        
        public void ShowCharacterPanel()
        {
            PanelMgr.PushPanel(PanelName.BoxPanel);
        }

        public void ShowOption()
        {
            homeOptionUi.SetVisible(true);
        }

        public void ShowRoomTypeUi()
        {
            if (!roomTypeUi.activeSelf)
                roomTypeUi.SetActive(true);
        }

        public void HideRoomTypeUi()
        {
            if (roomTypeUi.activeSelf)
                roomTypeUi.SetActive(false);
        }

        public void TryMatchSingleMode()
        {
            HideRoomTypeUi();
            LogicProxy.Instance.StartMatchSingle(GlobalVar.Uname);
        }

        public void TryMatchMultiMode()
        {
            HideRoomTypeUi();
            LogicProxy.Instance.StartMatchMulti(GlobalVar.Uname);
        }

        public void TryStoryMode()
        {
            HideRoomTypeUi();
            // LogicProxy.Instance.StartStoryMode(GlobalVar.Uname);
            SceneMgr.LoadSceneWithSimpleLoadingPanel(SceneName.MainCity);
        }

        public void UpdateUserInfo()
        {
            userInfoRect.UpdateInfo(
                AssetConstUtil.GetUserIcon(GlobalVar.Uface),
                GlobalVar.Unick,
                GlobalVar.Ulevel,
                GlobalVar.Uexp);
            coinBarInfo.SetValue(GlobalVar.Ucoin);
            diamondBarInfo.SetValue(GlobalVar.Udiamond);
        }
    }
}