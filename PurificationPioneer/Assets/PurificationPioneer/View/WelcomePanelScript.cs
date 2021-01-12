using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class WelcomePanelScript:MonoBehaviour
    {
        public InputField account;
        public InputField password;
        public Button loginBtn;
        public Button registerBtn;
        public Button exitBtn;

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
    }
}