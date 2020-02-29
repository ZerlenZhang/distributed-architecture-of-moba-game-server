using Moba.Protocol;
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

			userName = GetComponent<InputField>("LoginUi/Dlg/UserKey");
			pwdName = GetComponent<InputField>("LoginUi/Dlg/Pwd");

			userName.text = "zerlen";
			pwdName.text = "123";

			Assert.IsTrue(userName && pwdName);
			
			add_button_listener("GuestLoginBtn",AuthServiceProxy.Instance.GuestLogin);
			add_button_listener("LoginUi/Dlg/TextBtn", Login);
		}

		private void Login()
		{
			if (string.IsNullOrEmpty(userName.text)
			    || string.IsNullOrEmpty(pwdName.text))
				return;
			AuthServiceProxy.Instance.UserLogin(userName.text, pwdName.text);
		}
	}
}
