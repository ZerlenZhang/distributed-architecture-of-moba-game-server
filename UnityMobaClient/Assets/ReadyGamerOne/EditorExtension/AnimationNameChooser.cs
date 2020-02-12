using System;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [Serializable]
    public class AnimationNameChooser
    {
        public const float LabelWidth=0.4f;
        public const float ObjectFieldWidth = 0.2f;
        
        [SerializeField] private RuntimeAnimatorController rac;
        [SerializeField] private int selectedIndex;
        [SerializeField] private string stringValue;

        public string StringValue
        {
            get
            {
                if (rac == null)
                    throw new Exception("动画状态机为空");
                return stringValue;
            }
        }
    }
}