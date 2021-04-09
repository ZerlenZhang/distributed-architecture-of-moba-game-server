using System;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
#if UNITY_EDITOR
    
using UnityEditor;
#endif
using UnityEngine;

namespace DialogSystem.Model
{
    [Serializable]
    public class VarUnitInfo
    {
        public string VarName;
        public ArgChooser ArgChooser;

        public ArgType ArgType => ArgChooser.argType;
        public bool BoolValue => ArgChooser.BoolArg;
        public int IntValue => ArgChooser.IntArg;
        public float FloatValue => ArgChooser.FloatArg;
        public string StringValue => ArgChooser.StringArg;
        public Vector3 Vector3Value => ArgChooser.Vector3Arg;

        public void SetValue(int value)
        {
            ArgChooser.IntArg = value;
        }
        public void SetValue(string value)
        {
            ArgChooser.StringArg = value;
        }       
        public void SetValue(float value)
        {
            ArgChooser.FloatArg = value;
        }
        public void SetValue(bool value)
        {
            ArgChooser.BoolArg = value;
        }
        public void SetValue(Vector3 value)
        {
            ArgChooser.Vector3Arg = value;
        }

#if UNITY_EDITOR
        public void DrawWithRect(Rect rect)
        {
            var index = 0;
            this.VarName = EditorGUI.TextField(rect.GetRectAtIndex(index++), "变量名",this.VarName);
            switch (ArgChooser.argType)
            {
                case ArgType.Bool:
                    ArgChooser.BoolArg = EditorGUI.Toggle(rect.GetRectAtIndex(index++),"BoolArg", ArgChooser.BoolArg);
                    break;
                case ArgType.Float:
                    ArgChooser.FloatArg = EditorGUI.FloatField(rect.GetRectAtIndex(index++),"FloatArg", ArgChooser.FloatArg);

                    break;
                case ArgType.Int:
                    ArgChooser.IntArg = EditorGUI.IntField(rect.GetRectAtIndex(index++),"IntArg", ArgChooser.IntArg);
                    break;
                case ArgType.String:
                    ArgChooser.StringArg = EditorGUI.TextField(rect.GetRectAtIndex(index++),"StringArg", ArgChooser.StringArg);
                    break;
            } 
        }        
#endif
        

    }
}