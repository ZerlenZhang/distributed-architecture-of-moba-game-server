using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class StoryUnitUi : MonoBehaviour
    {
        public Text tipText;
        public Text titleText;
        public Text contentText;

        public RectTransform lockFlagGo;
        public void Set(string title, string content, int needTrustValue, bool isLock)
        {
            titleText.text = title;
            contentText.text = content;
            tipText.text = $"需要提高信赖至{needTrustValue}";
            lockFlagGo.gameObject.SetActive(isLock);
            contentText.gameObject.SetActive(!isLock);
        }
    }
}