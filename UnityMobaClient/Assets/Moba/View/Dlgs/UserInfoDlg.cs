using Moba.Global;
using Moba.Script;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Moba.View.Dlgs
{
    public class UserInfoDlg:MonoBehaviour
    {
        public InputField unickEditor;
        public GameObject upgrader;
        public Image avatorImag;
        public Button avatorBtn;

        public Toggle manToggle;
        public Toggle womanToggle;


        public GameObject avtorChooser;
        public Button closeBtn;


        private int uface;
        private int usex;
        
        private void OnEnable()
        {
            upgrader.SetActive(NetInfo.isGuest);
            unickEditor.text = NetInfo.unick;
            this.uface = NetInfo.uface;
            this.usex = NetInfo.usex;
            print("this.usex: " + this.usex);
            avatorImag.sprite = ResourceMgr.GetAsset<Sprite>("Avator_" + this.uface);
            womanToggle.isOn = this.usex == 1;
        }
        
        

        public void OnClose()
        {
            Destroy(this.gameObject);
        }

        public void OnClickIcon()
        {
            if (!avtorChooser.activeSelf)
                avtorChooser.SetActive(true);
        }

        public void OnClickMask()
        {
            if(avtorChooser.activeSelf)
                avtorChooser.SetActive(false);
        }
        
        public void OnSexChanged(int value)
        {
            this.usex = value;
        }

        public void OnIconChanged(int index)
        {
            this.uface = index;
            avatorImag.sprite = ResourceMgr.GetAsset<Sprite>("Avator_" + this.uface);
            OnClickMask();
        }

        public void OnRegister()
        {
            
        }

        public void OnUnRegister()
        {
            
        }

        public void OnConfirmInfo()
        {
            if (this.unickEditor.text.Length <= 0)
            {
                OnClose();
                return;
            }

            Debug.Log(this.unickEditor.text + " " + this.usex + " " + this.uface + " ");
            
            OnClose();
            MobaMgr.Instance.EditProfile(this.unickEditor.text, this.uface, this.usex);
        }
    }
}