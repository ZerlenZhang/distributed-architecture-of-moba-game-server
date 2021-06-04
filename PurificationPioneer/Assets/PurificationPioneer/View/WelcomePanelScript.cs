using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Script;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class WelcomePanelScript:MonoBehaviour
    {
        public InputField account;
        public InputField password;
        public RegisterUi registerUi;
        public Button loginBtn;
        public Button registerBtn;
        public Button exitBtn;

        public Image onImage;
        public Image offImage;

        public void Init()
        {
            registerUi.SetVisible(false);
        }

        public void TryLogin()
        {
            var account = this.account.text;
            var pwd = password.text;
            GlobalPref.Account = account;
            GlobalPref.Pwd = pwd;
            AuthProxy.Instance.Login(account,pwd);
        }

        public void TryExit()
        {
            Application.Quit();
        }

        public void OnEnableAudio(bool value)
        {
            offImage.gameObject.SetActive(!value);
            
            AudioMgr.Instance.BgmVolume = value ? 1 : 0;
            AudioMgr.Instance.EffectVolume = value ? 1 : 0;
        }
    }
}