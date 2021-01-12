using UnityEngine;
using PurificationPioneer.Scriptable;

namespace PurificationPioneer.Script
{
    public interface IPpController:IFrameSyncCharacter
    {
        CharacterConfigAsset CharacterConfig { get; }
        void InitCharacterController(int seatId, Vector3 logicPos, CharacterConfigAsset config);
    }
}