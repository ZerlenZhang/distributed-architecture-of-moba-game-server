using System.Collections.Generic;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [CustomPropertyDrawer(typeof(TransformPathChooser))]
    public class TransformPathChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indexProp = property.FindPropertyRelative("selectedIndex");
            var goProp = property.FindPropertyRelative("go");
            var obj = goProp.objectReferenceValue as GameObject;
            var nowPath = property.FindPropertyRelative("path");
            var paths = new List<string>();

            EditorGUI.LabelField(position.GetLeft(TransformPathChooser.LabelWidth), property.displayName);
            goProp.objectReferenceValue = EditorGUI.ObjectField(
                position.GetLeft(TransformPathChooser.LabelWidth + TransformPathChooser.ObjectFidldWidth,
                    TransformPathChooser.LabelWidth + 0.015f), goProp.objectReferenceValue, typeof(GameObject), false);
            var enumWidth = 1 - TransformPathChooser.LabelWidth - TransformPathChooser.ObjectFidldWidth;

            if (obj == null)
            {
                EditorGUI.HelpBox(position.GetRight(enumWidth), "请选择预制体", MessageType.Error);
                return;
            }

            Search(obj.transform, paths, "");


            if (paths.Count == 0)
            {
                EditorGUI.HelpBox(position.GetRight(enumWidth), "此物体没有子物体", MessageType.Error);
                return;
            }


            if (indexProp.intValue >= paths.Count)
            {
                Debug.LogError("路径索引越界，路径已经重置，请重新选择路径" + property.serializedObject.targetObject.name);
                indexProp.intValue = 0;
            }

            EditorGUI.BeginChangeCheck();

            indexProp.intValue = EditorGUI.Popup(position.GetRight(enumWidth), indexProp.intValue, paths.ToArray());

            var changed = EditorGUI.EndChangeCheck();
            if (string.IsNullOrEmpty(nowPath.stringValue))
                nowPath.stringValue = paths[indexProp.intValue];
            else
            {
                if (nowPath.stringValue != paths[indexProp.intValue])
                {
                    if (changed)
                    {
                        //主动修改
                        nowPath.stringValue = paths[indexProp.intValue];
                    }
                    else
                    {
                        //如果原先路径和当前路径不同
                        var currentName = nowPath.stringValue;
                        if (currentName.Contains("/"))
                        {
                            var select = currentName.LastIndexOf('/') + 1;
                            currentName = currentName.Substring(select, currentName.Length - select);
                        }

                        var index = 0;
                        foreach (var VARIABLE in paths)
                        {
                            //找到新路径
                            if (VARIABLE.EndsWith(currentName))
                            {
                                indexProp.intValue = index;
                                nowPath.stringValue = VARIABLE;
                                Debug.Log("重新找到！");
                                return;
                            }

                            index++;
                        }

                        Debug.LogError("路径丢失，没有找到！错误路径：" + nowPath.stringValue);
                    }

                }
            }
        }

        private void Search(Transform parent, List<string> paths,string nowPath)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (string.IsNullOrEmpty(nowPath))
                {
                    paths.Add(child.name);
                    Search(child,paths,child.name);
                }
                else
                {
                    paths.Add(nowPath + "/" + child.name);
                    Search(child, paths, nowPath + "/" + child.name);
                }
            }
        }
        
        
        
        
        
        
        
        
        
        
        
    }
}