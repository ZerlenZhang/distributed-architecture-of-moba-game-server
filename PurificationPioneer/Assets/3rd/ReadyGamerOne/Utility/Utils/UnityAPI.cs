using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class UnityAPI
    {
        public static void LockMouse(bool visible=false,bool lockPos=true)
        {
            Cursor.visible = visible;
            Cursor.lockState = lockPos ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public static void FreeMouse()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}