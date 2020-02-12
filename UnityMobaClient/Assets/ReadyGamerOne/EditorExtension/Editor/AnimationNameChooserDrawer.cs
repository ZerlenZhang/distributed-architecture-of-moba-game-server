using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [CustomPropertyDrawer(typeof(AnimationNameChooser))]
    public class AnimationNameChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position.GetLeft(AnimationNameChooser.LabelWidth), property.displayName);
            var racProp = property.FindPropertyRelative("rac");
            racProp.objectReferenceValue= EditorGUI.ObjectField(
                position.GetLeft(AnimationNameChooser.LabelWidth + AnimationNameChooser.ObjectFieldWidth,
                    AnimationNameChooser.LabelWidth+0.015f), racProp.objectReferenceValue, typeof(RuntimeAnimatorController),
                false);

            var enumWidth = 1 - AnimationNameChooser.LabelWidth - AnimationNameChooser.ObjectFieldWidth;
            var rac = racProp.objectReferenceValue as RuntimeAnimatorController;

            if (rac == null)
            {
                EditorGUI.HelpBox(position.GetRight(enumWidth), "请选择状态机", MessageType.Error);
                return;
            }
            
            var names = new string[rac.animationClips.Length];
            for (var i = 0; i < rac.animationClips.Length; i++)
                names[i] = rac.animationClips[i].name;
            var indexProp = property.FindPropertyRelative("selectedIndex");
            indexProp.intValue = EditorGUI.Popup(position.GetRight(enumWidth), indexProp.intValue, names);

            property.FindPropertyRelative("stringValue").stringValue = names[indexProp.intValue];

        }
    }
}