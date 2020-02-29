using Moba.Global;
using Moba.Protocol;
using UnityEngine;

namespace Moba.View.Dlgs
{
    public class LoginBonuesUi : MonoBehaviour
    {
        public GameObject recvBtn;

        public GameObject[] fingerPrints;
        public void ShowLoginBonues(int days)
        {
            print(days);
            int i;
            for ( i = 0; i < days; i++)
            {
                this.fingerPrints[i].SetActive(true);
            }

            for (; i < 5; i++)
            {
                this.fingerPrints[i].SetActive(false);
            }


            this.recvBtn.SetActive(NetInfo.gameInfo.bonues_status == 0);
        }

        public void OnRecvLoginBonuesClick()
        {
            SystemServiceProxy.Instance.RecvLoginBonues();
            Destroy(this.gameObject);
        }


        public void OnClose()
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        
    }
}