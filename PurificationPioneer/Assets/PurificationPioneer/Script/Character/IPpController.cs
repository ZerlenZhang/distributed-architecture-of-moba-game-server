using UnityEngine;
using PurificationPioneer.Scriptable;

namespace PurificationPioneer.Script
{
    public interface IPpController:IFrameSyncCharacter
    {
        HeroConfigAsset HeroConfig { get; }
        void InitCharacterController(int seatId, HeroConfigAsset config);
        
        Transform transform { get; }
    }
    
    public enum CharacterState
    {
        Move = 1,
        Free = 2,
        Idle = 3,
        Attack1 = 4,
        Attack2 = 5,
        Attack3 = 6,
        Skill1 = 7,
        Skill2 = 8,
        Die = 9,
    }
}