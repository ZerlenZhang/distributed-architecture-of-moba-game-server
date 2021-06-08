using System;
using DialogSystem.Model;

namespace DialogSystem.Scripts
{
    public class ActivateBetweenProgressRange : ActivateWithBoolVars
    {
        public ProgressPointRange ProgressPointRange;

        protected override Action<float> howActivateGameObject => (value) =>
        {
            if (value > ProgressPointRange.Min && value < ProgressPointRange.Max)
            {
                this.gameObject.SetActive(true);
            }
            else
                this.gameObject.SetActive(false);
        };
    }
}