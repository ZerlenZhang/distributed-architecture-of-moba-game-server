using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class LoadingProgressRect : MonoBehaviour
    {
        public Text progressText;
        public Slider progressSlider;

        public void SetProgress(float value)
        {
            value = Mathf.Clamp01(value);
            progressText.text = (value * 100).ToString(CultureInfo.InvariantCulture);
            progressSlider.value = value;
        }
    }
}