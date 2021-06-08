using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// Home界面右上角金币和钻石信息
    /// </summary>
    public class ItemBar : MonoBehaviour
    {
        public Button iconBtn;
        public Text countText;
        
        public void SetValue(int value)
        {
            countText.text = value.ToString();
        }

        public void UpdateValue(int change)
        {
            countText.text = (int.Parse(countText.text) + change).ToString();
        }
    }
}