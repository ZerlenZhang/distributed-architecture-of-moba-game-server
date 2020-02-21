using Moba.Protocol;
using UnityEngine;

namespace Moba.View.Dlgs
{
    public class LoginBonuesUi : MonoBehaviour
    {

        public GameObject[] fingerPrints;
        public void ShowLoginBonues(int days)
        {
            int i;
            for ( i = 0; i < days; i++)
            {
                this.fingerPrints[i].SetActive(true);
            }

            for (; i < 5; i++)
            {
                this.fingerPrints[i].SetActive(false);
            }
        }

        public void OnRecvLoginBonuesClick()
        {
            this.gameObject.SetActive(false);
            SystemServiceProxy.Instance.RecvLoginBonues();
        }


        public void OnClose()
        {
            this.gameObject.SetActive(false);
        }
        
    }
}