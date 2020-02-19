using Moba.Script;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Moba.View
{
	public partial class LoginPanel
	{
		private InputField userName;
		private InputField pwdName;
		partial void OnLoad()
		{
			//do any thing you want

			userName = view["LoginUi/Dlg/UserKey"].GetComponent<InputField>();
			pwdName = view["LoginUi/Dlg/Pwd"].GetComponent<InputField>();

			Assert.IsTrue(userName && pwdName);
			
			add_button_listener("GuestLoginBtn",MobaMgr.Instance.GuestLogin);
			add_button_listener("LoginUi/Dlg/TextBtn", Login);
		}

		private void Login()
		{
			if (string.IsNullOrEmpty(userName.text)
			    || string.IsNullOrEmpty(pwdName.text))
				return;
			MobaMgr.Instance.UserLogin(userName.text, pwdName.text);
		}
	}
}
