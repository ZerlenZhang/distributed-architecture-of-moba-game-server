using System.Collections.Generic;
using DialogSystem.ScriptObject;
using ReadyGamerOne.EditorExtension;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(BoolVarChooser))]
    public class BoolVarChooserDrawer:PropertyDrawer
    {
        private List<string> names=new List<string>();
        private List<int> indexs=new List<int>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            names.Clear();
            indexs.Clear();
            
            var index = 0;
            foreach (var VARIABLE in DialogVarAsset.Instance.varInfos)
            {
                if (VARIABLE.ArgType == ArgType.Bool)
                {
                    indexs.Add(index);
                    names.Add(VARIABLE.VarName);
                }
                index++;
            }

            var indexProp = property.FindPropertyRelative("selectedIndex");
            indexProp.intValue = EditorGUI.Popup(position, property.displayName, indexProp.intValue, names.ToArray());

        }
    }
}