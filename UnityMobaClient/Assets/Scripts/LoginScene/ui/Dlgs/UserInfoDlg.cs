using System;
using Moba.Protocol;
using Moba.Const;
using Moba.Global;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace Moba.View.Dlgs
{
    public class UserInfoDlg:MonoBehaviour
    {
        public InputField unickEditor;
        public GameObject upgrader;
        public Image avatorImag;

        public Toggle manToggle;
        public Toggle womanToggle;


        public GameObject avtorChooser;
        public GameObject registerUi;

        public InputField userNameEditor;
        public InputField userPwdEditor;

        private int uface;
        private int usex;

        private void Start()
        {
            //监听事件
            CEventCenter.AddListener<int>(Message.UpgradeGuest,OnUpgradeGuest);
            CEventCenter.AddListener(Message.Unregister,OnUnregister);
        }

        private void OnDestroy()
        {
            CEventCenter.RemoveListener<int>(Message.UpgradeGuest,OnUpgradeGuest);
            CEventCenter.RemoveListener(Message.Unregister,OnUnregister);
        }
        private void OnUnregister()
        {
            PanelMgr.PopPanel();
        }

        private void OnUpgradeGuest(int status)
        {
            if (status == Responce.Ok)
            {
                this.OnHideRegister();
                this.upgrader.SetActive(false);
            }
        }
        
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
        
        
        public void OnShowRegister()
        {
            registerUi.SetActive(true);
        }

        public void OnRegister()
        {
            if (!NetInfo.isGuest)
                return;
            if (string.IsNullOrEmpty(userNameEditor.text))
                return;
            if (string.IsNullOrEmpty(userNameEditor.text))
                return;

            var pwdMd5 = SecurityUtil.Md5(this.userPwdEditor.text);

            AuthServiceProxy.Instance.UpgradeGuest(this.userNameEditor.text, pwdMd5);
            
            //OnHideRegister();
        }

        public void OnHideRegister() 
        {
            if (registerUi == null)
            {
                throw new Exception("????");
            }
            registerUi.SetActive(false);
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
        

        public void OnUnRegister()
        {
            AuthServiceProxy.Instance.Unregister();
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
            AuthServiceProxy.Instance.EditProfile(this.unickEditor.text, this.uface, this.usex);
        }
    }
}