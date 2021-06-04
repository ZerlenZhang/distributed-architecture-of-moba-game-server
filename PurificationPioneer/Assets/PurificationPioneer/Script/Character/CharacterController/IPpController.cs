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
}