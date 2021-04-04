using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Const;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class TipMgr : GlobalMonoSingleton<TipMgr>
    {
        public GameObject m_TipPrefab;
        public float m_MoveDistance = 75;
        public float m_MoveTime = 2.5f;


        protected override void OnStateIsNull()
        {
            base.OnStateIsNull();
            CEventCenter.AddListener<int>(Message.OnResponseError,OnResponseError);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CEventCenter.RemoveListener<int>(Message.OnResponseError, OnResponseError);
        }

        private void OnResponseError(int errorCode)
        {
            switch (errorCode)
            {
                case Response.InvalidOperate:
                    Tip("非法操作");
                    break;
                case Response.InvalidParams:
                    Tip("非法参数");
                    break;
                case Response.SystemError:
                    Tip("系统错误");
                    break;
                case Response.UnameHasExist:
                    Tip("用户名已经存在");
                    break;
                case Response.UserIsFreeze:
                    Tip("账户已被冻结");
                    break;
                case Response.UnameOrPwdError:
                    Tip("用户名或密码错误");
                    break;
                case Response.Ok:
                    Tip("这不是挺好吗");
                    break;
            }
        }

        public void Tip(string msg)
        {
            var tip = Instantiate(m_TipPrefab, GlobalVar.CanvasComponent.transform);
            var tipUi = tip.GetComponent<TipUi>();
            Assert.IsTrue(tipUi);
            tipUi.Show(msg);

            DoAnimation(tipUi);
        }

        private void DoAnimation(TipUi tipUi)
        {
            var speed = m_MoveDistance / m_MoveTime;
            var timer = 0f;
            MainLoop.Instance.UpdateForSeconds(
                () =>
                {
                    if (tipUi == null)
                    {
                        return;
                    }
                    tipUi.transform.position += speed * Time.deltaTime * Vector3.up;
                    timer += Time.deltaTime;
                    tipUi.m_CanvasGroup.alpha = 1 - timer/m_MoveTime;
                },
                m_MoveTime,
                ()=>
                {
                    if(tipUi)
                        Destroy(tipUi.gameObject);
                });
        }
    }
}