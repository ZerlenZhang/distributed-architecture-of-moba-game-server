using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class DefaultCharacterController : 
        PpCharacterController
    {
        // [Tooltip("枪口位置和自身中心位置的便宜距离")]
        // public float offsetLen = 1;
        // [Tooltip("最小攻击间隔")]
        // public float attackMinDeltaTime = 0.5f;
        // private float lastAttackTime = 0;
        // [Tooltip("使用的武器ID，按说应该是匹配界面选择的，这里我们先直接指定了")]
        // public int weaponId = 0;
        // public WeaponConfigAsset WeaponConfig { get; protected set; }
        //
        // protected override void InitCharacter(bool isLocal)
        // {
        //     base.InitCharacter(isLocal);
        //     lastAttackTime = Time.realtimeSinceStartup;
        //     WeaponConfig = ResourceMgr.Instantiate<WeaponConfigAsset>(
        //         AssetConstUtil.GetWeaponConfigKey(weaponId));
        // }
        //
        // protected override void OnCommonAttack(int faceX,int faceY, int faceZ)
        // {
        //     var currentTime = Time.realtimeSinceStartup;
        //     if (currentTime - lastAttackTime > attackMinDeltaTime)
        //     {
        //         lastAttackTime = currentTime;
        //         //attack
        //         var shotDir = new Vector3(faceX, faceY, faceZ).normalized;
        //         var shotPoint = centerPoint.position + shotDir * offsetLen;
        //         WeaponConfig.CommonAttack(shotPoint, shotDir);
        //     }
        //
        // }
    }
}