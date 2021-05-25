using DG.Tweening;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.View
{
    public class MatchPanelScript : MonoBehaviour
    {
        public RectTransform leftGroup;
        public RectTransform rightGroup;
        public HeroInfoRectUi heroInfoRectUi;
        public HeroOptionRectUi heroOptionRectUi;
        public HeroChooseTimerUi heroChooseTimerUi;

        public void OnSubmit()
        {
            
            var time = GameSettings.Instance.MatchPanelEaseTime;
            var ease = GameSettings.Instance.MatchPanelEaseType;
            heroChooseTimerUi.OnSubmit(time, ease);
            heroOptionRectUi.gameObject.SetActive(false);
            heroInfoRectUi.transform.DOMove(heroChooseTimerUi.transform.position, time).SetEase(ease);
        }
    }
}