using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class AndroidBattleAttackBtnAreaUi : MonoBehaviour
    {
        public SkillSlotUi heroFirstSkillSlotUi;
        public SkillSlotUi heroSecondSkillSlotUi;
        public SkillSlotUi weaponCommonAttackSlotUi;
        public SkillSlotUi weaponFirstSkillSlotUi;
        public SkillSlotUi weaponSecondSkillSlotUi;
        
        public void Init()
        {
            var localHeroConfig = GlobalVar.LocalHeroConfig;
            var localWeaponConfig = ResourceMgr.GetAsset<WeaponConfigAsset>(AssetConstUtil.GetWeaponConfigKey(GlobalVar.LocalWeaponId));
            Assert.IsTrue(localHeroConfig.firstSkill
                          && localHeroConfig.secondSkill
                          && localWeaponConfig.firstSkill
                          && localWeaponConfig.commonAttack);

            heroFirstSkillSlotUi.Init(localHeroConfig.firstSkill,
                () => InputMgr.heroFirstSkill = true);
            heroSecondSkillSlotUi.Init(localHeroConfig.secondSkill,
                () => InputMgr.heroSecondSkill = true);

            weaponCommonAttackSlotUi.Init(localWeaponConfig.commonAttack,
                () => InputMgr.attack = true);
            
            weaponFirstSkillSlotUi.Init(localWeaponConfig.firstSkill,
                () => InputMgr.weaponFirstSkill = true);
            if (!localWeaponConfig.secondSkill)
            {
                weaponSecondSkillSlotUi.gameObject.SetActive(false);
            }
            else
            {
                weaponSecondSkillSlotUi.Init(localWeaponConfig.secondSkill,
                    () => InputMgr.weaponSecondSkill = true);
            }
        }
    }
}