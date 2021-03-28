using UnityEngine;

namespace PurificationPioneer.Script
{
    public class HomeOptionUi : MonoBehaviour
    {
        public void SetVisible(bool state)
        {
            gameObject.SetActive(state);
        }

        public void Init()
        {
            SetVisible(false);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}