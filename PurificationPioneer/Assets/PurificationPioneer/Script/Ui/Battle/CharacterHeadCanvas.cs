using System;
using ReadyGamerOne.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class CharacterHeadCanvas : MonoBehaviour
    {
        public Canvas canvas;
        public SuperBloodBar bloodBar;
        public Text playerNickText;

        public void Init(Camera eventCamera, string playerNick)
        {
            canvas.worldCamera = eventCamera;
            playerNickText.text = playerNick;
        }

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}