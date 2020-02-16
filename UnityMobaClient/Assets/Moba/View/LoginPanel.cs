using Moba.Script;

namespace Moba.View
{
	public partial class LoginPanel
	{
		partial void OnLoad()
		{
			//do any thing you want
			add_button_listener("GuestLoginBtn",MobaMgr.Instance.GuestLogin);
		}
	}
}
