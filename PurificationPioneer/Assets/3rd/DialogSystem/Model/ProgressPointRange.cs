using System;
using UnityEngine;

namespace DialogSystem.Model
{
    [Serializable]
    public class ProgressPointRange
    {
#pragma warning disable 649
        [SerializeField] private ProgressPointChooser min;
        [SerializeField] private ProgressPointChooser max;

        public float Min => Mathf.Min(min.Value, max.Value);
        public float Max => Mathf.Max(min.Value, max.Value);
#pragma warning restore 649
    }
}