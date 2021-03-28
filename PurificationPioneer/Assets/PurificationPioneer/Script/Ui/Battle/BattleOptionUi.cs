using System;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
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

            Cursor.visible = state;
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void ExitGame()
        {
            LogicProxy.Instance.TryExitGame(GlobalVar.Uname);
        }
    }
}