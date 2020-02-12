using System;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyGamerOne.Scripts.Editor
{
    [CustomEditor(typeof(SuperBloodBar))]
    public class SuperBloodBarEditor:UnityEditor.Editor
    {
        private SerializedProperty transitionTimeProp;
        private SerializedProperty transitionImageProp;
        private SerializedProperty transitionColorProp;

        private SerializedProperty fillImageProp;
        private SerializedProperty fillColorProp;

        private SerializedProperty backGroundImageProp;
        private SerializedProperty backGroundColorProp;

        private SerializedProperty maxValueProp;
        private SerializedProperty valueProp;

        private SerializedProperty onValueChangeProp;


        private SuperBloodBar bloodBar;

        private void OnEnable()
        {
            this.bloodBar=target as SuperBloodBar;

            transitionTimeProp = serializedObject.FindProperty("transitionTimeScaler");
            transitionImageProp = serializedObject.FindProperty("transitionImage");
            if (transitionImageProp.objectReferenceValue == null)
                transitionImageProp.objectReferenceValue = GetComponentFromTransform<Image>(bloodBar.transform, "Transition");
            
            transitionColorProp = serializedObject.FindProperty("transitionColor");
            fillImageProp = serializedObject.FindProperty("fillImage");
            if (fillImageProp.objectReferenceValue == null)
                fillImageProp.objectReferenceValue = GetComponentFromTransform<Image>(bloodBar.transform, "Fill");
            fillColorProp = serializedObject.FindProperty("fillColor");
            backGroundImageProp = serializedObject.FindProperty("backGroundImage");
            if (backGroundImageProp.objectReferenceValue == null)
                backGroundImageProp.objectReferenceValue =
                    GetComponentFromTransform<Image>(bloodBar.transform, "BackGround");
            backGroundColorProp = serializedObject.FindProperty("backGroundColor");
            maxValueProp = serializedObject.FindProperty("maxValue");
            valueProp = serializedObject.FindProperty("value");
            onValueChangeProp = serializedObject.FindProperty("onValueChange");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(fillImageProp);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(fillColorProp);
            if (EditorGUI.EndChangeCheck())
            {
                var alpha = bloodBar.FillColor.a;
                bloodBar.FillColor = fillColorProp.colorValue;
                bloodBar.fillImage.SetAlpha(alpha);
            }
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(transitionTimeProp);
            EditorGUILayout.PropertyField(transitionImageProp);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(transitionColorProp);
            if (EditorGUI.EndChangeCheck())
            {
                var alpha = bloodBar.TransitionColor.a;
                bloodBar.TransitionColor = transitionColorProp.colorValue;
                bloodBar.transitionImage.SetAlpha(alpha);

            }
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(backGroundImageProp);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(backGroundColorProp);
            if (EditorGUI.EndChangeCheck())
            {
                var alpha = bloodBar.BackGroundColor.a;
                bloodBar.BackGroundColor = backGroundColorProp.colorValue;
                bloodBar.BackGroundColor.SetAlpha(alpha);
            }
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(maxValueProp);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Slider(valueProp, 0, maxValueProp.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                bloodBar.Value = valueProp.floatValue;
            }

            serializedObject.ApplyModifiedProperties();
        }


        private T GetComponentFromTransform<T>(Transform from, string path)
        where T:MonoBehaviour
        {
            if (from == null)
                throw new Exception("当前物品为空");
            var obj = from.Find(path);
            if (obj == null)
                return null;

            return obj.GetComponent<T>();
        }
    }
}