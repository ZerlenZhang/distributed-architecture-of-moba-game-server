using DialogSystem.Model;
using DialogSystem.View;

namespace PurificationPioneer.Dialog
{
    public class PpCaptionWordUi:AbstractWordDialogUI
    {
        public PpCaptionWordUi(DialogUnitInfo unitInfo) : base(unitInfo)
        {
        }

        protected override string SpeakerTextPath => "SpeakerName";
        protected override string CaptionWordTextPath => "bg/Word";
    }
}