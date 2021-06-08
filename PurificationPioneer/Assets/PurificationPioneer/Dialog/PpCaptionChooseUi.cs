using System;
using DialogSystem.Model;
using DialogSystem.View;
using PurificationPioneer.Const;
using UnityEngine;

namespace PurificationPioneer.Dialog
{
    public class PpCaptionChooseUi:AbstractChooseDialogUI
    {
        public PpCaptionChooseUi(DialogUnitInfo dialogUnitInfo) : base(dialogUnitInfo)
        {
        }

        protected override string SpeakerTextPath => "SpeakerName";
        protected override string TitleTextPath => "bg/Title";
        protected override Func<Transform> onGetChoiceParent => () => m_TransFrom.Find("BtnList");
        protected override string choiceObjPath => DialogName.ChoiseBtn;
        protected override string textPathOnChoice => "Button/Text";
    }
}