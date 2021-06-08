using DialogSystem.ScriptObject;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(CharacterChooser))]
    public class CharacterChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var selectedIndexProp = property.FindPropertyRelative("selectedIndex");
            selectedIndexProp.intValue = EditorGUI.Popup(position,"说话人", selectedIndexProp.intValue,
                DialogCharacterAsset.Instance.characterNames.ToArray());
        }
    }
}