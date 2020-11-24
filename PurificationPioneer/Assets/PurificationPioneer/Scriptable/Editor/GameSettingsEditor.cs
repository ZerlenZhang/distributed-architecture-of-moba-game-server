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
            if (GUILayout.Button("显示ip"))
            {
                (target as GameSettings)?.LogIp();
            }
        }
    }
}