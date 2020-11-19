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
        private RectTransform selfRect;
        private void OnEnable()
        {
            this.bloodBar=target as SuperBloodBar;
            selfRect = bloodBar.GetComponent<RectTransform>();

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

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(fillImageProp);
            if (EditorGUI.EndChangeCheck())
            {
                var image = fillImageProp.objectReferenceValue as Image;
                var rect = image.GetComponent<RectTransform>();
                rect.name = "Fill";
                rect.SetParent(bloodBar.transform,false);
                var size = selfRect.rect.size;
                size.x *= selfRect.pivot.x-0.5f;
                size.y *= selfRect.pivot.y-0.5f;
                rect.localPosition=-size;
                rect.anchorMin=Vector2.zero;
                rect.anchorMax=Vector2.one;
                rect.pivot = new Vector2(0, 0.5f);
                rect.sizeDelta=Vector2.zero;
                fillColorProp.colorValue=Color.green;
                image.color = fillColorProp.colorValue;
            }
            
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
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(transitionImageProp);
            if (EditorGUI.EndChangeCheck())
            {
                var image = transitionImageProp.objectReferenceValue as Image;
                var rect = image.GetComponent<RectTransform>();
                rect.SetParent(bloodBar.transform,false);
                rect.name = "Transition";
                var size = selfRect.rect.size;
                size.x *= selfRect.pivot.x-0.5f;
                size.y *= selfRect.pivot.y-0.5f;
                rect.localPosition=-size;
                rect.anchorMin=Vector2.zero;
                rect.anchorMax=Vector2.one;
                rect.pivot = new Vector2(0, 0.5f);
                rect.sizeDelta=Vector2.zero;
                transitionColorProp.colorValue=Color.yellow;
                image.color = transitionColorProp.colorValue;
            }
            
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(transitionColorProp);
            if (EditorGUI.EndChangeCheck())
            {
                var alpha = bloodBar.TransitionColor.a;
                bloodBar.TransitionColor = transitionColorProp.colorValue;
                bloodBar.transitionImage.SetAlpha(alpha);

            }
            EditorGUILayout.Separator();
            

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(backGroundImageProp);
            if (EditorGUI.EndChangeCheck())
            {
                var image = backGroundImageProp.objectReferenceValue as Image;
                var rect = image.GetComponent<RectTransform>();
                rect.name = "BackGround";
                rect.SetParent(bloodBar.transform,false);
                
                var size = selfRect.rect.size;
                size.x *= selfRect.pivot.x-0.5f;
                size.y *= selfRect.pivot.y-0.5f;
                rect.localPosition=-size;
                
                rect.anchorMin=Vector2.zero;
                rect.anchorMax=Vector2.one;
                rect.sizeDelta=Vector2.zero;
                backGroundColorProp.colorValue = Color.gray;
                image.color = backGroundColorProp.colorValue;
            }
            
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