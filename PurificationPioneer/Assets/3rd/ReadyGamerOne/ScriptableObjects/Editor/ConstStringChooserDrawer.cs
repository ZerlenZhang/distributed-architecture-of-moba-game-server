using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.ScriptableObjects.Editor
{
    [CustomPropertyDrawer(typeof(ConstStringChooser))]
    public class ConstStringChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indexProp = property.FindPropertyRelative("selectedIndex");
            indexProp.intValue = EditorGUI.Popup(position, property.displayName, indexProp.intValue,
                PrefUtil.Instance.constStrings.ToArray());
        }
    }
}