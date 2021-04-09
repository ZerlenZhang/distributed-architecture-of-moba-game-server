using System;
using DialogSystem.ScriptObject;
#if UNITY_EDITOR
    
using UnityEditor;
#endif
using UnityEngine;

namespace DialogSystem.Model
{
    [Serializable]
    public class CharacterChooser
    {
        [SerializeField] private int selectedIndex=0;

        public string CharacterName
        {
            get
            {
                var arr = DialogCharacterAsset.Instance.characterNames;
                if (selectedIndex >= arr.Count)
                    throw new Exception("CharacterChooser selectedIndex 异常：" + selectedIndex);
                return arr[selectedIndex];
            }
        }

#if UNITY_EDITOR
        public static string GetShowTextFromSerializedProperty(SerializedProperty property)
        {
            var selectedIndex = property.FindPropertyRelative("selectedIndex").intValue;
            var arr = DialogCharacterAsset.Instance.characterNames;
            if (selectedIndex >= arr.Count)
            {
                if (selectedIndex == 0)
                    return null;
                throw new Exception("CharacterChooser selectedIndex 异常：" + selectedIndex+"  arr.Count:"+arr.Count);
            }
            return arr[selectedIndex];
        }        
#endif

    }
}