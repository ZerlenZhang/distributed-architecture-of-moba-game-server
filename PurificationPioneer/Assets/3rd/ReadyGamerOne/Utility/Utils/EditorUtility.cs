using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.Windows;
#endif
using UnityEngine;

namespace ReadyGamerOne.Utility
{
#if UNITY_EDITOR
    public class EditorUtil:Editor
    {

        /// <summary>
        /// 显示一块可以接收拖拽物体的区域
        /// </summary>
        /// <param name="meg">tips</param>
        /// <param name="height">高度</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetDragObjectsFromArea<T>(string meg = null,float height=35f)
            where T: UnityEngine.Object
        {
            Event aEvent;
            aEvent = Event.current;

            GUI.contentColor = Color.white;
            List<T> list = new List<T>();

            var dragArea = GUILayoutUtility.GetRect(0f, height, GUILayout.ExpandWidth(true));

            GUIContent title = new GUIContent(meg);
            if (string.IsNullOrEmpty(meg))
            {
                title = new GUIContent("Drag Object here from Project view to get the object");
            }
            GUIStyle s = new GUIStyle()
            {
                fontSize=15,    
                alignment = TextAnchor.MiddleCenter
            };

            GUI.Box(dragArea,title,s);

            switch (aEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragArea.Contains(aEvent.mousePosition))
                    {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (aEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
                        {

                            var temp = DragAndDrop.objectReferences[i] as T;
                            if(temp!=null)
                                list.Add(temp);
                        }
                    }

                    Event.current.Use();
                    break;
                default:
                    break;
            }

            return list;
        }
        public static List<T> GetDragObjectsFromArea<T>(Rect dragArea)
            where T : UnityEngine.Object
        {
            Event aEvent;
            aEvent = Event.current;

            GUI.contentColor = Color.white;
            List<T> list = new List<T>();
            switch (aEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragArea.Contains(aEvent.mousePosition))
                    {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (aEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i)
                        {

                            var temp = DragAndDrop.objectReferences[i] as T;
                            if (temp != null)
                                list.Add(temp);
                        }
                    }

                    Event.current.Use();
                    break;
                default:
                    break;
            }

            return list;
        }
        
        /// <summary>
        /// 根据Type里的public static 字段填充arrProp
        /// </summary>
        /// <param name="arrProp"></param>
        /// <param name="type"></param>
        public static void InitSerializedStringArray(SerializedProperty arrProp, Type type)
        {
            arrProp.arraySize = 0;
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                arrProp.InsertArrayElementAtIndex(i);
                arrProp.GetArrayElementAtIndex(i).stringValue = fieldInfos[i].GetValue(null) as string;
            }
        }

        /// <summary>
        /// 根据list填充arrProp
        /// </summary>
        /// <param name="arrProp"></param>
        /// <param name="strs"></param>
        public static void InitSerializedStringArray(SerializedProperty arrProp, IList<string> strs)
        {
            arrProp.arraySize = 0;
            for (int i = 0; i < strs.Count; i++)
            {
                arrProp.InsertArrayElementAtIndex(i);
                arrProp.GetArrayElementAtIndex(i).stringValue = strs[i];

            }
        }
        
        /// <summary>
        /// 安全的创建ScriptableObject资源
        /// </summary>
        /// <param name="defaultName"></param>
        /// <typeparam name="T"></typeparam>
        public static void CreateAsset<T>(string defaultName)
            where T:ScriptableObject
        {
            var strs = Selection.assetGUIDs;

            var path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var name = path + "/"+defaultName;
            while (File.Exists(name + ".asset"))
                name += "(1)";
            

            AssetDatabase.CreateAsset(CreateInstance<T>(), name + ".asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<T>(name + ".asset");
        }


        public static Type GetType(string fullTypeName)
        {
            Assert.IsFalse(string.IsNullOrEmpty(fullTypeName));
            return Type.GetType(fullTypeName);
        }
    }    
#endif

}
