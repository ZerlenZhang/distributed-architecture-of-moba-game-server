using System;
using DialogSystem.ScriptObject;

namespace DialogSystem.Model
{
    [Serializable]
    public class SkipChoice
    {
        public ValueChooser enable;
        public string text;
        public AbstractDialogInfoAsset targetDialogInfoAsset;
        public bool willBack=true;
    }
}