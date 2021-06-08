using System.Collections.Generic;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Network.Proxy;
using UnityEditor;
using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    [CustomEditor(typeof(GameSettings))]
    public class GameSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("测试Udp"))
            {
                LogicProxy.Instance.TestUdp("hello world");
            }
            if (GUILayout.Button("Try_NextFrameInput"))
            {
                LogicProxy.Instance.SendLogicInput(
                    666,555,444,333,new List<PlayerInput>
                    {
                        new PlayerInput
                        {
                            seatId = 444,
                            attack = false,
                            jump = false,
                            mouseX = 222,
                            mouseY = 111,
                            moveX = 0,
                            moveY = 0,
                        }
                    });
            }
        }
    }
}