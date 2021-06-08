using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace PurificationPioneer.Script
{
    public class AndroidBattleAttackBtnAreaUi : MonoBehaviour
    {
        [FormerlySerializedAs("weaponCommonAttackSlotUi")] public SkillSlotUi commonAttackSlotUi;
        
        public void Init()
        {
            var localHeroConfig = GlobalVar.LocalHeroConfig;

            commonAttackSlotUi.Init(localHeroConfig.attackIcon,
                () => InputMgr.attack = true,
                continueTrigger:true);
        }
    }
}