using System;
using UnityEngine;
using UnityEngine.UI;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine.EventSystems;

namespace PurificationPioneer.Script
{
    public class SkillSlotUi:MonoBehaviour
    {
        public Image image;
        private Func<bool> skillTrigger;
        private Action onUseSkill;

        private bool startWork = false;

        private bool m_TriggerSkillByContinue = false;

        public void Init(Sprite icon, Action skill, Func<bool> ifUseSkill = null,
            bool continueTrigger = false)
        {
            image.gameObject.SetActive(true);
            image.sprite = icon;
            this.skillTrigger = ifUseSkill;
            onUseSkill = skill;
            startWork = true;

            if (continueTrigger)
            {
                var uiHelper = image.GetOrAddComponent<UIInputer>();
                uiHelper.eventOnPointerDown -= OnTriggerSkill;
                uiHelper.eventOnPointerDown += OnTriggerSkill;

                uiHelper.eventOnPointerUp -= OnFinishSkill;
                uiHelper.eventOnPointerUp += OnFinishSkill;
            }


            void OnTriggerSkill(PointerEventData evt)
            {
                m_TriggerSkillByContinue = true;
            }

            void OnFinishSkill(PointerEventData evt)
            {
                m_TriggerSkillByContinue = false;
            }
        }

        public void UseSkill()
        {
            onUseSkill?.Invoke();
        }

        private void Update()
        {
            if (!startWork)
                return;

            if (skillTrigger!=null && skillTrigger())
                UseSkill();

            if (m_TriggerSkillByContinue)
            {
                UseSkill();
            }
        }
    }
}