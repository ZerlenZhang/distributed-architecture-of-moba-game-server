using System;
using System.Collections;
using PurificationPioneer.Const;
using PurificationPioneer.View;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PurificationPioneer.Script
{
    public static class SceneMgr
    {
	    public static Coroutine LoadScene(string sceneName, Action<int> onLoading)
	    {
		    return MainLoop.Instance.StartCoroutine(LoadSceneAsync(sceneName, onLoading));
	    }

	    public static Coroutine LoadSceneWithSimpleLoadingPanel(string sceneName)
	    {
		    PanelMgr.PushPanel(PanelName.SimpleLoadingPanel);
		    if (PanelMgr.CurrentPanel is SimpleLoadingPanel panel)
		    {
			    return MainLoop.Instance.StartCoroutine(LoadSceneAsync(sceneName, 
				    progress=>panel.SetProgress(progress/100f)));			    
		    }

		    throw new Exception("CurrentPanel isn't SimpleLoadingPanel");
	    }
        
		private static IEnumerator LoadSceneAsync(string sceneName, Action<int> onLoading)
		{
			//显示用的进度
			int displayPro = 0;
            
			//实际的进度
			int realPro = 0;
            
			//开始加载下一个场景
			var async = SceneManager.LoadSceneAsync(sceneName);
            
			//暂时不进入下一个产经
			async.allowSceneActivation = false;
			
			//在加载不足90%时，进度条缓慢增长动画
			var speed = 1;
			while (async.progress < 0.9f)
			{
				realPro = (int) (async.progress * 100.0f);
                
				//如果显示进度尚未达到实际进度，每帧增加1%
				while (displayPro < realPro)
				{
					displayPro += speed;

					if (displayPro >= realPro)
					{
						speed = 1;
						displayPro = realPro;
					}
					else
						speed <<= 1;
					
                    
                    
					onLoading?.Invoke(displayPro);
					yield return new WaitForEndOfFrame();
				}
			}
            
			//加载最后一段
			realPro = 100;
			while (displayPro < realPro)
			{					
				displayPro += speed;

				if (displayPro >= realPro)
				{
					speed = 1;
					displayPro = realPro;
				}
				else
					speed <<= 1;
                
				onLoading?.Invoke(displayPro);
                
				yield return new WaitForEndOfFrame();
			}
			
            
			//允许场景切换
			async.allowSceneActivation = true;
            
			//等待场景真正加载完毕
			while (!async.isDone)
			{
				yield return new WaitForEndOfFrame();
			}
		}
    }
}