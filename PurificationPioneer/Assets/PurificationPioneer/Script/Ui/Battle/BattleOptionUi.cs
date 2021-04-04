using System;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class BattleOptionUi : MonoBehaviour
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

        public void ExitGame()
        {
            LogicProxy.Instance.TryExitGame(GlobalVar.Uname);
        }
    }
}