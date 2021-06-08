using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class RegisterUi : MonoBehaviour
    {
        public InputField m_Unick;
        public InputField m_Account;
        public InputField m_Pwd;
        public InputField m_ConfirmPwd;

        public Button m_SignUpBtn;

        public void SetVisible(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Register()
        {
            if (string.IsNullOrEmpty(m_Unick.text))
            {
                TipMgr.Instance.Tip("昵称为空");
                return;
            }

            if (string.IsNullOrEmpty(m_Account.text))
            {
                TipMgr.Instance.Tip("账号为空");
                return;
            }

            if (string.IsNullOrEmpty(m_Pwd.text))
            {
                TipMgr.Instance.Tip("密码为空");
                return;
            }

            if (m_Pwd.text != m_ConfirmPwd.text)
            {
                TipMgr.Instance.Tip("两次输入密码不一致");
                return;
            }

            GlobalPref.Account = m_Account.text;
            GlobalPref.Pwd = m_Pwd.text;

            AuthProxy.Instance.Register(
                m_Account.text,
                m_Pwd.text,
                m_Unick.text);
        }
        
        
        
    }
}