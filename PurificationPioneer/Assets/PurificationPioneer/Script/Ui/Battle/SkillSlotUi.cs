using System;
using UnityEngine;
using UnityEngine.UI;
using PurificationPioneer.Scriptable;

namespace PurificationPioneer.Script
{
    public class SkillSlotUi:MonoBehaviour
    {
        public Text count;
        public Image mask;
        public Image image;

        private Func<bool> skillTrigger;
        private float adder = 0;
        private float cd;
        private Action onUseSkill;
        public int CurrentCount => int.Parse(count.text);
        
        private int maxCount;

        private bool startWork = false;

        public void Init(SkillConfigAsset skillConfig, Action skill,Func<bool> skillTrigger=null)
        {
            image.gameObject.SetActive(true);
            image.sprite = skillConfig.icon;
            count.text = "0";
            this.skillTrigger = skillTrigger;
            this.cd = skillConfig.cd;
            this.maxCount = skillConfig.maxCount;
            if (this.maxCount == 1)
                this.count.gameObject.SetActive(false);
            onUseSkill = skill;
            startWork = true;
        }

        public void UseSkill()
        {
            if (CurrentCount < 1)
                return;
            count.text = (CurrentCount - 1).ToString();
            onUseSkill?.Invoke();
        }

        private void Update()
        {
            if (!startWork)
                return;

            if (skillTrigger!=null && skillTrigger())
                UseSkill();
            
            if (CurrentCount == maxCount)
                return;

            adder += Time.deltaTime;

            mask.fillAmount = 1 - adder / cd;
            
            if (adder >= cd)
            {
                count.text = (CurrentCount + 1).ToString();
                adder = 0;
            }
        }
    }
}