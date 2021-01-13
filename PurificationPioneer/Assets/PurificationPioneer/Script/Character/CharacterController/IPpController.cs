using UnityEngine;
using PurificationPioneer.Scriptable;

namespace PurificationPioneer.Script
{
    public interface IPpController:IFrameSyncCharacter
    {
        HeroConfigAsset HeroConfig { get; }
        void InitCharacterController(int seatId, Vector3 logicPos, HeroConfigAsset config);
    }
}