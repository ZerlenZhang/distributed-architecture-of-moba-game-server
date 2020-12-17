using System;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public static class InputMgr
    {
        private static bool jump = false;
        private static bool attack = false;
        private static int mouseX = 0;
        private static int mouseY = 0;
        private static int moveX = 0;
        private static int moveY = 0;

        public static PlayerInput GetInput()
        {
            var input = new PlayerInput();
            input.seatId = GlobalVar.SeatId;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            input.attack = attack;
            input.jump = jump;
            input.mouseX = mouseX;
            input.mouseY = mouseY;
            input.moveX = Input.GetAxis("Horizontal").ToInt();
            input.moveY = Input.GetAxis("Vertical").ToInt();
            
#elif UNITY_ANDROID
            input.attack = attack;
            input.jump = jump;
            input.mouseX = mouseX;
            input.mouseY = mouseY;
            CEventCenter.BroadMessage<Action<Vector2>>(Message.AndroidMoveInput,
                (moveInput) =>
                {
                    input.moveX = moveInput.x.ToInt();
                    input.moveY = moveInput.y.ToInt();
                });
#endif
            return input;
        }
    }
}