using System;
using System.Linq;
using DialogSystem.ScriptObject;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
#if UNITY_EDITOR
    
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;

namespace DialogSystem.Model
{
    [Serializable]
#pragma warning disable 660,661
    public class ValueChooser
#pragma warning restore 660,661
    {
#pragma warning disable 649
        #region Static

#if UNITY_EDITOR
        public static int GetArgTypeFromSerializedProperty(SerializedProperty property)
        {
            var valueTypeProp = property.FindPropertyRelative("valueType");
            
            var argChooserProp = property.FindPropertyRelative("ArgChooser");

            SerializedProperty targetProp = null;
            
            switch (valueTypeProp.enumValueIndex)
            {
                case 0:// Value
                    targetProp = argChooserProp;
                    break;
                case 1://var
                    targetProp = property;
                    break;
            }

            return targetProp.FindPropertyRelative("argType").intValue;
        }
        
        public static string GetShowTextFromSerializedProperty(SerializedProperty property)
        {

            var argChooserProp = property.FindPropertyRelative("ArgChooser");
            
            switch (property.FindPropertyRelative("valueType").enumValueIndex)
            {
                case 0://value
                    return ReadyGamerOne.EditorExtension.ArgChooser.GetShowTextFromSerializedProperty(argChooserProp);
                case 1://var
                    var asset =
                        property.FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue as
                            AbstractDialogInfoAsset;
                    if (asset == null)
                        throw new Exception("asset is null");
                    var text = asset.GetValueStrings(
                        (ReadyGamerOne.EditorExtension.ArgType) property.FindPropertyRelative("argType")
                            .enumValueIndex).Keys.ToArray()[property.FindPropertyRelative("selectedIndex").intValue];
                    return text;
            }
            throw new Exception("???");
        }

        public static ArgType GetArgType(SerializedProperty property)
        {
            switch (property.FindPropertyRelative("valueType").enumValueIndex)
            {
                case 0://Value
                    return (ArgType) (property.FindPropertyRelative("ArgChooser").FindPropertyRelative("argType").enumValueIndex);
                case 1://Var
                    return (ArgType) property.FindPropertyRelative("argType").enumValueIndex;
            }
            throw new Exception("类型异常");
        }        
#endif

        
        public static float valueTypeWidth = 0.2f;
        public static float argTypeWidth = 0.3f;

        #endregion
        
        #region Static

        #endregion
        public enum ValueType
        {
            Value,
            Var
        }
        
        public ValueType valueType;
        
        public AbstractDialogInfoAsset abstractAbstractDialogInfoAsset;
        
        [SerializeField] private ArgChooser ArgChooser;
        [SerializeField] private int selectedIndex;
        [SerializeField] private ArgType argType;

        
        
        public ArgType ArgType
        {
            get
            {
                if (valueType == ValueType.Value)
                    return ArgChooser.argType;
                return argType;
            }
        }
        public int IntValue
        {
            get
            {
                if (valueType == ValueType.Var)
                {
                    try
                    {
                        var str= abstractAbstractDialogInfoAsset.GetValueStrings(ArgType.Int).Values.ToArray()[selectedIndex];
                        return Convert.ToInt32(str);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception(abstractAbstractDialogInfoAsset.name+"   Index:"+selectedIndex);
                    }
                }
                return ArgChooser.IntArg;
            }
        }
        public Vector3 Vector3Value
        {
            get
            {
                if (valueType == ValueType.Var)
                {
                    try
                    {
                        var str= abstractAbstractDialogInfoAsset.GetValueStrings(ArgType.Vector3).Values.ToArray()[selectedIndex];
                        return ConvertUtil.String2Vector3(str);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception(abstractAbstractDialogInfoAsset.name+"   Index:"+selectedIndex);
                    }

                }
                return ArgChooser.Vector3Arg;
            }
        }
        public float FloatValue
        {
            get
            {
                if (valueType == ValueType.Var)
                {
                    try
                    {
                        var str= abstractAbstractDialogInfoAsset.GetValueStrings(ArgType.Float).Values.ToArray()[selectedIndex];
                        return Convert.ToSingle(str);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception(abstractAbstractDialogInfoAsset.name+"   Index:"+selectedIndex);
                    }

                }
                return ArgChooser.FloatArg;
            }
        }
        public bool BoolValue
        {
            get
            {
                if (valueType == ValueType.Var)
                {
                    try
                    {
                        var str= abstractAbstractDialogInfoAsset.GetValueStrings(ArgType.Bool).Values.ToArray()[selectedIndex];
                        return Convert.ToBoolean(str);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception(abstractAbstractDialogInfoAsset.name+"   Index:"+selectedIndex);
                    }

                }
                return ArgChooser.BoolArg;
            }
        }
        public string StrValue
        {
            get
            {
                if (valueType == ValueType.Var)
                {
                    try
                    {
                        var str= abstractAbstractDialogInfoAsset.GetValueStrings(ArgType.String).Values.ToArray()[selectedIndex];
                        return str;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception(abstractAbstractDialogInfoAsset.name+"   Index:"+selectedIndex);
                    }

                }
                return ArgChooser.StringArg;
            }
        }

        #region 运算符重载

        public static bool operator ==(ValueChooser a1, ValueChooser a2)
        {
            Assert.IsTrue(a1.ArgType==a2.ArgType);

            switch (a1.ArgType)
            {
                case ArgType.Bool:
                    return a1.BoolValue == a2.BoolValue;
                case ArgType.Int:
                    return a1.IntValue == a2.IntValue;
                case ArgType.String:
                    return a1.StrValue == a2.StrValue;
            }

            throw new Exception("ValueChooser 比较异常");
        }

        public static bool operator !=(ValueChooser a1, ValueChooser a2)
        {
            return !(a1 == a2);
        }

        public static bool operator >(ValueChooser a1, ValueChooser a2)
        {
            
            Assert.IsTrue((a1.ArgType==ArgType.Int||a1.ArgType==ArgType.Float
                           )&&(a2.ArgType==ArgType.Int||a2.ArgType==ArgType.Float
                           ));
            
            switch (a1.ArgType)
            {   
                case ArgType.Float:
                    return a1.FloatValue > a2.FloatValue;
                case ArgType.Int:
                    return a1.IntValue > a2.IntValue;
            }

            throw new Exception("ValueChooser 比较异常");
        }

        public static bool operator <(ValueChooser a1, ValueChooser a2)
        {
            Assert.IsTrue((a1.ArgType==ArgType.Int||a1.ArgType==ArgType.Float
                          )&&(a2.ArgType==ArgType.Int||a2.ArgType==ArgType.Float
                          ));
            
            switch (a1.ArgType)
            {   
                case ArgType.Float:
                    return a1.FloatValue < a2.FloatValue;
                case ArgType.Int:
                    return a1.IntValue < a2.IntValue;
            }

            throw new Exception("ValueChooser 比较异常");
        }

        public static bool operator >=(ValueChooser a1, ValueChooser a2)
        {
            Assert.IsTrue((a1.ArgType==ArgType.Int||a1.ArgType==ArgType.Float
                          )&&(a2.ArgType==ArgType.Int||a2.ArgType==ArgType.Float
                          ));
            
            switch (a1.ArgType)
            {   
                case ArgType.Float:
                    return a1.FloatValue >= a2.FloatValue;
                case ArgType.Int:
                    return a1.IntValue >= a2.IntValue;
            }

            throw new Exception("ValueChooser 比较异常");
        }

        public static bool operator <=(ValueChooser a1, ValueChooser a2)
        {
            Assert.IsTrue((a1.ArgType==ArgType.Int||a1.ArgType==ArgType.Float
                          )&&(a2.ArgType==ArgType.Int||a2.ArgType==ArgType.Float
                          ));
            
            switch (a1.ArgType)
            {   
                case ArgType.Float:
                    return a1.FloatValue <= a2.FloatValue;
                case ArgType.Int:
                    return a1.IntValue <= a2.IntValue;
            }

            throw new Exception("ValueChooser 比较异常");
        }        

        #endregion


#pragma warning restore 649
    }
}