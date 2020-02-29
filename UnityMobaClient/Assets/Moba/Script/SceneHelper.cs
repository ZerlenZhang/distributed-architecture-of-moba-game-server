using System;
using Moba.Const;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.View;
using UnityEngine;

namespace Moba.Script
{
    public class SceneHelper : MonoBehaviour
    {
        public StringChooser startPanel=new StringChooser(typeof(PanelName));

        protected virtual void Start()
        {
            PanelMgr.PushPanel(startPanel.StringValue);
        }
    }
}