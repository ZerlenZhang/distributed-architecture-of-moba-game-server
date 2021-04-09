using System;
using DialogSystem.ScriptObject;
using UnityEngine;

namespace DialogSystem.Model
{
    [Serializable]
    public class BoolVarChooser
    {
#pragma warning disable 649
        [SerializeField] private int selectedIndex;
#pragma warning restore 649
        public bool Value => DialogVarAsset.Instance.varInfos[selectedIndex].BoolValue;
    }
}