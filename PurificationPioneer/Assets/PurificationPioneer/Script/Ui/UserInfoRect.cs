using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// Home界面左上角显示当前用户状态的Ui
    /// </summary>
    public class UserInfoRect : MonoBehaviour
    {
        public Image iconImage;
        public Button iconBtn;
        public Text nickText;
        public Slider expSlider;
        public Text levelText;

        public void UpdateInfo(Sprite icon, string nick, int level, int exp)
        {
            iconImage.sprite = icon;
            nickText.text = nick;
            levelText.text = level.ToString();
        }
    }
}