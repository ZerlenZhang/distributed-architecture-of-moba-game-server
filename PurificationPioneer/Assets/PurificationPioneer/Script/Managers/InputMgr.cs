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

        public static bool jump = false;
        public static bool attack = false;
        public static bool heroFirstSkill = false;
        public static bool heroSecondSkill = false;
        public static bool weaponFirstSkill = false;
        public static bool weaponSecondSkill = false;

        public static PlayerInput GetInput()
        {
            var cameraDir = LocalCameraHelper.Instance.GetCameraDirection();
            var input = new PlayerInput
            {
                seatId = GlobalVar.LocalSeatId,
                faceX = cameraDir.x.ToInt(),
                faceY = cameraDir.y.ToInt(),
                faceZ = cameraDir.z.ToInt(),
            };

            if (GlobalVar.IsPlayerInControl)
            {
                input.attack = attack;
                input.jump = jump;
                input.mouseX = mouseX;
                input.mouseY = mouseY;
                input.heroFirstSkill = heroFirstSkill;
                input.heroSecondSkill = heroSecondSkill;
                input.weaponFirstSkill = weaponFirstSkill;
                input.weaponSecondSkill = weaponSecondSkill;
            }

            if (GameSettings.Instance.EnableInputPredict)
            {
                CEventCenter.BroadMessage(Message.OnInputPredict, input);
            }

#if UNITY_EDITOR
            if (GameSettings.Instance.WorkAsAndroid)
            {           
                CEventCenter.BroadMessage<Action<Vector2>>(Message.AndroidMoveInput,
                (moveInput) =>
                {
                    if (!GlobalVar.IsPlayerInControl)
                        return;
                    input.moveX = moveInput.x.ToInt();
                    input.moveY = moveInput.y.ToInt();
                });
            }
            else
            {
                if (GlobalVar.IsPlayerInControl)
                {
                    input.moveX = Input.GetAxis("Horizontal").ToInt();
                    input.moveY = Input.GetAxis("Vertical").ToInt();                    
                }

            }
#elif UNITY_ANDROID
            CEventCenter.BroadMessage<Action<Vector2>>(Message.AndroidMoveInput,
                (moveInput) =>
                {                    
                    if (!GlobalVar.IsPlayerInControl)
                        return;
                    input.moveX = moveInput.x.ToInt();
                    input.moveY = moveInput.y.ToInt();
                });
#elif UNITY_STANDALONE_WIN                
            if (GlobalVar.IsPlayerInControl)
            {
                input.moveX = Input.GetAxis("Horizontal").ToInt();
                input.moveY = Input.GetAxis("Vertical").ToInt();
            }
#endif
            ClearInput();
            return input;
        }

        private static void ClearInput()
        {
            mouseX = 0;
            mouseY = 0;
            jump = false;
            attack = false;
            heroFirstSkill = false;
            heroSecondSkill = false;
            weaponFirstSkill = false;
            weaponSecondSkill = false;
        }
    }
}