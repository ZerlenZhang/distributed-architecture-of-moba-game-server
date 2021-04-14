using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class UnityAPI
    {
        private static bool s_IsLocked = false;
        public static bool IsLocked => s_IsLocked;
        public static void LockMouse(bool visible=false,bool lockPos=true)
        {
            s_IsLocked = true;
            Cursor.visible = visible;
            Cursor.lockState = lockPos ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public static void FreeMouse()
        {
            s_IsLocked = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}