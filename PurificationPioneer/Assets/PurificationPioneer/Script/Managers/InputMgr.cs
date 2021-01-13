using System;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public static class InputMgr
    {
        private static int mouseX = 0;
        private static int mouseY = 0;
        private static int moveX = 0;
        private static int moveY = 0;

        public static bool jump = false;
        public static bool attack = false;
        public static bool heroFirstSkill = false;
        public static bool heroSecondSkill = false;
        public static bool weaponFirstSkill = false;
        public static bool weaponSecondSkill = false;

        public static PlayerInput GetInput()
        {
            var input = new PlayerInput
            {
                seatId = GlobalVar.LocalSeatId,
                attack = attack,
                jump = jump,
                mouseX = mouseX,
                mouseY = mouseY,
                heroFirstSkill = heroFirstSkill,
                heroSecondSkill = heroSecondSkill,
                weaponFirstSkill = weaponFirstSkill,
                weaponSecondSkill = weaponSecondSkill
                
            };

#if UNITY_EDITOR
            if (GameSettings.Instance.WorkAsAndroid)
            {           
                CEventCenter.BroadMessage<Action<Vector2>>(Message.AndroidMoveInput,
                (moveInput) =>
                {
                    input.moveX = moveInput.x.ToInt();
                    input.moveY = moveInput.y.ToInt();
                });
            }
            else
            {
                input.moveX = Input.GetAxis("Horizontal").ToInt();
                input.moveY = Input.GetAxis("Vertical").ToInt();
            }
#elif UNITY_ANDROID
            CEventCenter.BroadMessage<Action<Vector2>>(Message.AndroidMoveInput,
                (moveInput) =>
                {
                    input.moveX = moveInput.x.ToInt();
                    input.moveY = moveInput.y.ToInt();
                });
#elif UNITY_STANDALONE_WIN
            input.moveX = Input.GetAxis("Horizontal").ToInt();
            input.moveY = Input.GetAxis("Vertical").ToInt();
            
#endif
            ClearInput();
            return input;
        }

        private static void ClearInput()
        {
            jump = false;
            attack = false;
            heroFirstSkill = false;
            heroSecondSkill = false;
            weaponFirstSkill = false;
            weaponSecondSkill = false;
        }
    }
}