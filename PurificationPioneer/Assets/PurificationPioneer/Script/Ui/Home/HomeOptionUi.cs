using PurificationPioneer.Network;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class HomeOptionUi : MonoBehaviour
    {
        public Slider bgm;
        public Slider effect;
        
        
        public void SetVisible(bool state)
        {
            if (state == gameObject.activeSelf)
                return;
            gameObject.SetActive(state);
        }

        public void Init()
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

        public void ExitGame()
        {
            NetworkMgr.Instance.Disconnect();
            Application.Quit();
        }
    }
}