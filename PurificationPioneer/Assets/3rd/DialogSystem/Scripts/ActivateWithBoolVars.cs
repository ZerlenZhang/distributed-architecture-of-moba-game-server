using System;
using System.Collections.Generic;
using DialogSystem.Model;
using DialogSystem.ScriptObject;

namespace DialogSystem.Scripts
{
    public class ActivateWithBoolVars : UnityEngine.MonoBehaviour
    {
        protected virtual Action<float> howActivateGameObject => process => gameObject.SetActive(true);
        protected virtual Action<float> howDisactivateGameObject => process => gameObject.SetActive(false);
        
        public List<BoolVarChooser> enableConditions = new List<BoolVarChooser>();

        protected virtual void Awake()
        {
            TriggerWithConditions(DialogProgressAsset.Instance.CurrentProgress);
            DialogProgressAsset.Instance.onProgressChanged += TriggerWithConditions;
        }

        protected virtual void OnDestroy()
        {
            DialogProgressAsset.Instance.onProgressChanged -= TriggerWithConditions;
        }

        private void TriggerWithConditions(float progress)
        {
            if (enableConditions.Count == 0)
            {
                //没有条件，直接调用
                howActivateGameObject(progress);
            }
            else
            {
                foreach (var VARIABLE in enableConditions)
                {
                    if (!VARIABLE.Value)
                    {
                        //有一个条件不满足，都不调用，直接禁用
                        howDisactivateGameObject(progress);
                        return;
                    }
                }

                howActivateGameObject(progress);
            }
        }
    }
}