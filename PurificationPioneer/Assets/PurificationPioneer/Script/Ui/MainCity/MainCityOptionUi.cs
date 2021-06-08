using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PurificationPioneer.Script
{
    public class MainCityOptionUi : MonoBehaviour
    {
        private void Start()
        {
            SetVisible(false);
        }

        
        public void SetVisible(bool state)
        {
            gameObject.SetActive(state);
#if UNITY_EDITOR
            if(!GameSettings.Instance.WorkAsAndroid)
            {
                if (state)
                {
                    PurificationPioneerMgr.Instance.m_NeedResetMouseMode = false;
                    UnityAPI.FreeMouse();
                }
                else
                {
                    UnityAPI.LockMouse();
                }
            }
#elif UNITY_STANDALONE_WIN
            if (state)
            {
                UnityAPI.FreeMouse();
            }
            else
            {
                UnityAPI.LockMouse();
            }
#endif
        }

        public void ExitStoryMode()
        {
            SceneMgr.LoadSceneWithSimpleLoadingPanel(SceneName.Home);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}