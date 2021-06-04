using System;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class BattleOptionUi : MonoBehaviour
    {
        public Slider bgm;
        public Slider effect;
        private void Start()
        {
            SetVisible(false);
            bgm.value = AudioMgr.Instance.BgmVolume;
            effect.value = AudioMgr.Instance.EffectVolume;
        }

        public void SetBgmVolume(float value)
        {
            AudioMgr.Instance.BgmVolume = bgm.value;
        }

        public void SetEffectVolume(float value)
        {
            AudioMgr.Instance.EffectVolume = effect.value;
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

        public void ExitRoom()
        {
            LogicProxy.Instance.TryExitGame(GlobalVar.Uname);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}