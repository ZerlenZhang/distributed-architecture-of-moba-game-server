using System;
using System.Collections;
using DialogSystem.Model;
using DialogSystem.Scripts;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DialogSystem.View
{
    public abstract class AbstractNarratorDialogUI:DialogUnitUI
    {
        protected bool enbaleGoNext = false;
        protected abstract string TextPath { get; }
        protected abstract string ImagePath { get; }

        protected Text text;
        protected Image image;

        protected AbstractNarratorDialogUI(DialogUnitInfo info):base(info)
        {
            Create(info.abstractAbstractDialogInfoAsset.NarratorUiKeys);

            text = m_TransFrom.Find(TextPath).GetComponent<Text>();
            text.text = info.wordToNarrator;

            image = m_TransFrom.Find(ImagePath).GetComponent<Image>();

            switch (info.m_NarratorType)
            {
                case NarratorType.Color:
                    image.sprite = null;
                    image.color = info.m_NarratorColor;
                    break;
                case NarratorType.Image:
                    image.sprite = info.m_NarratorImage;
                    image.color = info.m_NarratorColor;
                    break;
                case NarratorType.Object:
                    var obj = Object.Instantiate(info.m_NarratorObject, m_TransFrom.parent);
                    obj.transform.SetAsFirstSibling();
                    image.sprite = info.m_NarratorImage;
                    image.color = info.m_NarratorColor;
                    break;
            }
            
            
            Show();

            if (info.enableFadeOut)
            {
                text.gameObject.AddComponent<TypeWriterEffect>().StartWriter(
                    info.wordToNarrator.Length/info.narratorSpeed,
                    ()=> MainLoop.Instance.StartCoroutine(FadeOutText(text,info.fadeOutTime,()=>DestroyThis())),
                    ()=>canGoNext,
                    info.abstractAbstractDialogInfoAsset.affectedByTimeScale);
            }
            else
            {
                text.gameObject.AddComponent<TypeWriterEffect>().StartWriter(
                    info.wordToNarrator.Length/info.narratorSpeed,
                    ()=>enbaleGoNext=true,
                    ()=>canGoNext,
                    info.abstractAbstractDialogInfoAsset.affectedByTimeScale);                
            }
            

        }

        protected override void Update()
        {
            base.Update();
            if(enbaleGoNext && canGoNext)
                DestroyThis();
        }

        private IEnumerator FadeOutText(Text text, float fadeOutTime,Action endCall)
        {
            var color = text.color;
            for (var i = 0f; i < fadeOutTime; i += Time.deltaTime)
            {
                var alpha = 1 - i / fadeOutTime;
                text.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
            endCall?.Invoke();
        }
    }
}