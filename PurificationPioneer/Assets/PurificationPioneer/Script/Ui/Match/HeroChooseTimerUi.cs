using DG.Tweening;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class HeroChooseTimerUi : MonoBehaviour
    {
        public RectTransform timerObj;
        public Text timerText;
        public Button submitBtn;

        private void Start()
        {
            SetTime(GlobalVar.SelectHeroTime);
        }

        public void SubmitHeroReq()
        {
            if (GlobalVar.IsLocalSubmit)
            {
                Debug.LogError("重复锁定");
                return;
            }

            Debug.Log("Try Submit");
            LogicProxy.Instance.SubmitHero(GlobalVar.Uname);
        }

        public void SetTime(int time)
        {
            timerText.text = time.ToString();
        }


        public void OnSubmit(float time, Ease easeType)
        {
            this.submitBtn.gameObject.SetActive(false);
            this.timerObj.DOMove(this.submitBtn.transform.position, time).SetEase(easeType);
        }

    }
}