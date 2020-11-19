using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [CustomPropertyDrawer(typeof(ResourcesPathChooser))]
    public class ResourcesPathChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indexProp = property.FindPropertyRelative("selectedIndex");
            var nowPath = property.FindPropertyRelative("Path");
            var paths=new List<string>();

            var resPath = Application.dataPath + "/Resources";

            if (!Directory.Exists(resPath))
            {
                Directory.CreateDirectory(resPath);
                AssetDatabase.Refresh();
            }
            
            Search(resPath, paths);
            if (paths.Count==0)
            {
                EditorGUI.HelpBox(position, "现在Resources目录为空", MessageType.Error);
                return;
            }
            else if (indexProp.intValue >= paths.Count)
            {
                Debug.LogError("路径索引越界，路径已经重置，请重新选择路径" + property.serializedObject.targetObject.name);
                indexProp.intValue = 0;
            }
            EditorGUI.BeginChangeCheck();
            
            indexProp.intValue = EditorGUI.Popup(position,property.displayName, indexProp.intValue, paths.ToArray());

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
                            currentName = currentName.Substring(select, currentName.Length-select);
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
                        Debug.LogError("路径丢失，没有找到！错误路径："+nowPath.stringValue);                        
                    }

                }
            }
        }
        
        
        private void Search(string path, List<string> paths,string pathBefore="")
        {
            var info = new DirectoryInfo(path);
            foreach (var VARIABLE in info.GetFileSystemInfos())
            {
                if (VARIABLE is DirectoryInfo)
                {
                    if(string.IsNullOrEmpty(pathBefore))
                        Search(path + "/" + VARIABLE.Name, paths, VARIABLE.Name);
                    else
                        Search(path + "/" + VARIABLE.Name, paths, pathBefore + "/" + VARIABLE.Name);
                }
                else
                {
                    if (VARIABLE.Name.EndsWith(".meta"))
                        continue;

                    var okName = VARIABLE.Name.Substring(0, VARIABLE.Name.LastIndexOf('.'));

                    if (string.IsNullOrEmpty(pathBefore))
                        paths.Add(okName);
                    else
                        paths.Add(pathBefore + "/" + okName);
                }
            }
        }
    }
}