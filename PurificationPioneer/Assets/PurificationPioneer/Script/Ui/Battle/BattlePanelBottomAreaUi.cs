using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Scripts;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class BattlePanelBottomAreaUi : MonoBehaviour
    {
        public Image heroIcon;
        public SuperBloodBar bloodBar;
        public SkillSlotUi firstSkillSlotUi;
        public SkillSlotUi secondSkillSlotUi;
        public SkillSlotUi weaponFirstSkillSlotUi;
        public SkillSlotUi weaponSecondSkillSlotUi;

        public void Init()
        {
            var localHeroConfig = GlobalVar.LocalHeroConfig;
            var localWeaponConfig = ResourceMgr.GetAsset<WeaponConfigAsset>(AssetConstUtil.GetWeaponConfigKey(GlobalVar.LocalWeaponId));
            Assert.IsTrue(localHeroConfig.firstSkill
                          && localHeroConfig.secondSkill
                          && localWeaponConfig.firstSkill);

            heroIcon.sprite = localHeroConfig.icon;
            bloodBar.Value = 1;
             
            firstSkillSlotUi.Init(localHeroConfig.firstSkill,
                () => InputMgr.heroFirstSkill = true,
                () => Input.GetKeyDown(GameSettings.Instance.HeroFirstSkillKey));
            secondSkillSlotUi.Init(localHeroConfig.secondSkill,
                () => InputMgr.heroSecondSkill = true,
                () => Input.GetKeyDown(GameSettings.Instance.HeroSecondSkillKey));
            weaponFirstSkillSlotUi.Init(localWeaponConfig.firstSkill,
                () => InputMgr.weaponFirstSkill = true,
                () => Input.GetKeyDown(GameSettings.Instance.WeaponFirstSkillKey));
            if (!localWeaponConfig.secondSkill)
            {
                weaponSecondSkillSlotUi.gameObject.SetActive(false);
            }
            else
            {
                weaponSecondSkillSlotUi.Init(localWeaponConfig.secondSkill,
                    () => InputMgr.weaponSecondSkill = true,
                    () => Input.GetKeyDown(GameSettings.Instance.WeaponSecondSkillKey));
            }
        }
    }
}