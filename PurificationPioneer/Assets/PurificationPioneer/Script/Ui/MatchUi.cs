using System;
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

        public void OnCancel()
        {
            Debug.Log("Try Cancel");
        }
        
        public void StartMatch()
        {
            gameObject.SetActive(true);
        }

        public void StopMatch()
        {
            gameObject.SetActive(false);
        }
    }
}