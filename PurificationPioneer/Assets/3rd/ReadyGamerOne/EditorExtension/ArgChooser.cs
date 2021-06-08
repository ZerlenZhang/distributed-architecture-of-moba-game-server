using System;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [Serializable]
    public class ArgChooser
    {
        #region Static

        public static bool typeChangAble = true;
        public static float argTypeWidth = 0.4f;
#if UNITY_EDITOR
        public static string GetShowTextFromSerializedProperty(SerializedProperty property)
        {
            var text = "";

            var argTypeProp = property.FindPropertyRelative("argType");

            switch (argTypeProp.enumValueIndex)
            {
                case 0://int 
                    text = property.FindPropertyRelative("IntArg").intValue.ToString();
                    break;
                case 1://float
                    text = property.FindPropertyRelative("FloatArg").floatValue.ToString();
                    break;
                case 2://string
                    text = property.FindPropertyRelative("StringArg").stringValue;
                    break;
                case 3://bool
                    text = property.FindPropertyRelative("BoolArg").boolValue.ToString();
                    break;
                case 4://Vector3
                    text = property.FindPropertyRelative("Vector3Arg").vector3Value.ToString();
                    break;
            }

            return text;
        }
#endif



        public string GetValueString()
        {
            switch (argType)
            {
                case ArgType.Bool:
                    return BoolArg.ToString();
                case ArgType.Int:
                    return IntArg.ToString();
                case ArgType.Float:
                    return FloatArg.ToString();
                case ArgType.String:
                    return StringArg;
                case ArgType.Vector3:
                    return Vector3Arg.ToString();
            }

            return "???";
        }

        #endregion
        
        public ArgType argType;

        public bool BoolArg;
        public int IntArg;
        public float FloatArg;
        public string StringArg;
        public Vector3 Vector3Arg;

        public void SetValue(ArgChooser arg)
        {
            this.BoolArg = arg.BoolArg;
            this.IntArg = arg.IntArg;
            this.FloatArg = arg.FloatArg;
            this.StringArg = arg.StringArg;
            this.Vector3Arg = arg.Vector3Arg;
        }
        
    }
}