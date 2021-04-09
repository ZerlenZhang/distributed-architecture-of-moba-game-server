using PurificationPioneer.Network;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class HomeOptionUi : MonoBehaviour
    {
        public void SetVisible(bool state)
        {
            if (state == gameObject.activeSelf)
                return;
            gameObject.SetActive(state);
        }

        public void Init()
        {
            SetVisible(false);
        }

        public void ExitGame()
        {
            NetworkMgr.Instance.Disconnect();
            Application.Quit();
        }
    }
}