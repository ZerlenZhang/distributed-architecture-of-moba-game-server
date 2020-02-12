using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.Editor
{
    public class EditorUtil:UnityEditor.Editor
    {

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

        public static void InitSerializedStringArray(SerializedProperty arrProp, IList<string> strs)
        {
            arrProp.arraySize = 0;
            for (int i = 0; i < strs.Count; i++)
            {
                arrProp.InsertArrayElementAtIndex(i);
                arrProp.GetArrayElementAtIndex(i).stringValue = strs[i];

            }
        }

    }
}
