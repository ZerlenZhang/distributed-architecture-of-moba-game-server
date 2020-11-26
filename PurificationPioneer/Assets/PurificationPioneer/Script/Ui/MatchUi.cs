using System;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class MatchUi:MonoBehaviour
    {
        public Button cancelBtn;
        public Image timingImage;
        public Text playerCountText;

        private int current;
        private int max;
        public void OnCancel()
        {
            LogicProxy.Instance.TryStopMatch(GlobalVar.uname);
        }
        
        public void StartMatch(int cur,int max)
        {
            gameObject.SetActive(true);
            current = cur;
            this.max = max;
            playerCountText.text = $"{current}/{this.max}";
        }

        public void AddPlayer()
        {
            current++;
            playerCountText.text = $"{current}/{this.max}";
        }

        public void RemovePlayer()
        {
            current--;
            playerCountText.text = $"{current}/{this.max}";
        }

        public void StopMatch()
        {
            gameObject.SetActive(false);
        }
    }
}