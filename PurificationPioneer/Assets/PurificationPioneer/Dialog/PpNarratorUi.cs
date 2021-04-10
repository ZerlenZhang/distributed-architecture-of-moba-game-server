using DialogSystem.Model;
using DialogSystem.View;

namespace PurificationPioneer.Dialog
{
    public class PpNarratorUi:AbstractNarratorDialogUI
    {
        public PpNarratorUi(DialogUnitInfo info) : base(info)
        {
        }

        protected override string TextPath => "bg/Text";
        protected override string ImagePath => "bg";
    }
}