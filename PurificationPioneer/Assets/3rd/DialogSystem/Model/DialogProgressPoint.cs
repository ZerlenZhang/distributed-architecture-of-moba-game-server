using System;

namespace DialogSystem.Model
{
    /// <summary>
    /// 对话进度点
    /// </summary>
    [Serializable]
    public class DialogProgressPoint
    {
        public int index;
        public string name;
        public float value;
    }
}