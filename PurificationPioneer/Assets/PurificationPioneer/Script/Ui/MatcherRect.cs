using System.Collections;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Utility;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 英雄选择界面两侧标识双方选取英雄的Ui
    /// </summary>
    public class MatcherRect : MonoBehaviour
    {
        public Image heroIcon;
        public Text nickText;
        public Image bg;
        public int SeatId { get; private set; }
        public string Unick { get; private set; }
        public int HeroId { get; private set; }

        private Coroutine shineCoroutine;

        public void InitValues(GlobalVar.MatcherInfo matcherInfo)
        {
            SeatId = matcherInfo.SeatId;
            Unick = matcherInfo.Unick;
            nickText.text = matcherInfo.Unick;
            if (SeatId == GlobalVar.SeatId)
            {
                shineCoroutine = MainLoop.Instance.StartCoroutines(ShineBg());
            }
        }

        public void SelectHeroRes(SelectHeroRes res)
        {
            HeroId = res.hero_id;
            heroIcon.sprite = AssetConstUtil.GetHeroIcon(HeroId);
        }

        public void SubmitHeroRes()
        {
            Debug.Log($"[MatcherRect] {Unick} 锁定英雄 HeroId[{HeroId}]");
            if (Unick == GlobalVar.Unick && null!=shineCoroutine)
            {
                MainLoop.Instance.StopCoroutine(shineCoroutine);
                shineCoroutine = null;
            }
            bg.SetAlpha(0);
        }

        private void OnDestroy()
        {            
            if (Unick == GlobalVar.Unick && null!=shineCoroutine)
            {
                MainLoop.Instance.StopCoroutine(shineCoroutine);
                shineCoroutine = null;
            }
        }

        private IEnumerator ShineBg()
        {
            float target = 1;
            float src = 0;
            float timer = 0;
            float circleTime = 1;
            while (true)
            {
                yield return null;
                timer += Time.deltaTime;
                if (timer >= circleTime)
                {
                    timer = 0;
                    var temp = target;
                    target = src;
                    src = temp;
                }
                bg.SetAlpha(Mathf.Lerp(src, target, timer / circleTime));
            }
        }
    }
}