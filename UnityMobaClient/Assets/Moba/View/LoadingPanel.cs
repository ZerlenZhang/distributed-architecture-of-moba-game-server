using System.Collections;
using Moba.Const;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Moba.View
{
	public partial class LoadingPanel
	{
		private Image process;
		private AsyncOperation ao;

		partial void OnLoad()
		{
			//do any thing you want
			process = GetComponent<Image>("sliderbg/process");

			Assert.IsNotNull(process);

			process.fillAmount = 0;
		}

		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener<string>(Message.LoadSceneAsync,OnLoadScene);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener<string>(Message.LoadSceneAsync,OnLoadScene);
		}

		private void OnLoadScene(string obj)
		{
			MainLoop.Instance.StartCoroutine(LoadSceneAsync(obj));
		}

		private IEnumerator LoadSceneAsync(string sceneName)
		{
			ao = SceneManager.LoadSceneAsync(sceneName);
			ao.allowSceneActivation = false;
			yield return ao;
		}
		
		protected override void Update()
		{
			base.Update();
			if (ao == null)
				return;
			var per = ao.progress;
			if (per >= 0.9f)
			{
				ao.allowSceneActivation = true;
			}

			this.process.fillAmount = ao.progress / 0.9f;
		}
	}
}
