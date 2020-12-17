using UnityEngine;

namespace PurificationPioneer.Script
{
    public interface IPpController:IFrameSyncWithSeatId
    {
        void InitCharacterController(int seatId, Vector3 logicPos);
    }
}