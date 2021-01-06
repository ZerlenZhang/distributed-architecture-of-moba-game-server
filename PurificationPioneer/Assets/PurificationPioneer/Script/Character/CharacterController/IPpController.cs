using UnityEngine;

namespace PurificationPioneer.Script
{
    public interface IPpController:IFrameSyncCharacter
    {
        void InitCharacterController(int seatId, Vector3 logicPos);
    }
}