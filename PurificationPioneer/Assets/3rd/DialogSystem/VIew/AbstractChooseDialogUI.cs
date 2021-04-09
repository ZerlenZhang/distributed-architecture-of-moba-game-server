using System;
using System.Collections;
using DialogSystem.Model;
using DialogSystem.Scripts;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem.View
{
    public abstract class AbstractChooseDialogUI:DialogUnitUI
    {
        protected abstract string SpeakerTextPath { get; }
        protected abstract string TitleTextPath { get; }
        protected abstract Func<Transform> onGetChoiceParent { get; }
        protected abstract string choiceObjPath { get; }
        protected abstract string textPathOnChoice { get; }

        /// <summary>
        /// 有时间限制的时候随着时间流逝会调用这个委托
        /// <float>已经经过的时间</float>
        /// <float>最大时间限制</float>
        /// <GameObject>默认选项物体</GameObject>
        /// </summary>
        protected virtual Action<float, float, GameObject> onTimeLeave => null;
        private Transform choiceParent;

        public Transform ChoiceParent
        {
            get
            {
                if (choiceParent == null)
                    choiceParent = onGetChoiceParent();
                if (!choiceParent)
                    throw new Exception("获取 ChoiceParent 失败");
                return choiceParent;
            }
        }

        protected bool clicked = false;
        
        public AbstractChooseDialogUI(DialogUnitInfo dialogUnitInfo):base(dialogUnitInfo)
        {
            Create(dialogUnitInfo.abstractAbstractDialogInfoAsset.ChooseUiKeys);

            m_TransFrom.Find(SpeakerTextPath).GetComponent<Text>().text = dialogUnitInfo.SpeakerName;
            var speaker = m_TransFrom.Find(SpeakerTextPath).GetComponent<Text>();
            speaker.text = dialogUnitInfo.SpeakerName;
            var title= m_TransFrom.Find(TitleTextPath).GetComponent<Text>();
            title.text = dialogUnitInfo.title;

            Show();

            //打字机效果
            var titleEffect = title.gameObject.AddComponent<TypeWriterEffect>();
            titleEffect.StartWriter(
                dialogUnitInfo.abstractAbstractDialogInfoAsset.wordFinishTime,
                () => { MainLoop.Instance.StartCoroutine(ShowChoices(dialogUnitInfo)); },
                ()=>canGoNext,
                dialogUnitInfo.abstractAbstractDialogInfoAsset.affectedByTimeScale);
        }

        private IEnumerator ShowChoices(DialogUnitInfo dialogUnitInfo)
        {
            GameObject defaultObj = null;
            var index = 0;
            //显示选项
            foreach (var choice in dialogUnitInfo.Choices)
            {                
                if(choice.enable.BoolValue==false)
                                 continue;
                
                var obj = ResourceMgr.InstantiateGameObject(choiceObjPath, ChoiceParent);

                if (index++ == dialogUnitInfo.defaultIndex)
                    defaultObj = obj;
                

                var text = obj.transform.Find(textPathOnChoice).GetComponent<Text>();
                text.text = choice.text;
                
                //添加回调
                text.gameObject.AddComponent<UIInputer>().eventOnPointerClick += (dat) =>
                {
                    clicked = true;
                    
                    Hide();
                    if (choice.targetDialogInfoAsset == null)
                    {
                        CEventCenter.BroadMessage(Scripts.DialogSystem.ChooseNotBack,dialogUnitInfo.abstractAbstractDialogInfoAsset.name);
                        DestroyThis();
                    }
                    else
                    {
//                        Debug.Log("选项："+choice.text+"  willBack:"+choice.willBack);
                        if (!choice.willBack)
                        {
//                            Debug.Log($"广播消息：{Scripts.DialogSystem.ChooseNotBack},名字：{dialogUnitInfo.abstractAbstractDialogInfoAsset.name}");
                            CEventCenter.BroadMessage(Scripts.DialogSystem.ChooseNotBack,dialogUnitInfo.abstractAbstractDialogInfoAsset.name);
//                            Debug.Log("开始选项协程："+choice.targetDialogInfoAsset.name);
                            MainLoop.Instance.StartCoroutine(
                                Scripts.DialogSystem.RunDialogAwait(choice.targetDialogInfoAsset, () =>
                                {
                                    if (dialogUnitInfo.sendToExWord)
                                    {
                                        dialogUnitInfo.words = choice.text;
                                        CEventCenter.BroadMessage(Scripts.DialogSystem.ExternWord, dialogUnitInfo);
                                    }
                                    DestroyThis();
                                }));  
                        }
                        else
                        {
//                            Debug.Log("Else???");
                            MainLoop.Instance.StartCoroutine(
                                Scripts.DialogSystem.RunDialog(choice.targetDialogInfoAsset, () =>
                                {
                                    if (dialogUnitInfo.sendToExWord)
                                    {
                                        dialogUnitInfo.words = choice.text;
                                        CEventCenter.BroadMessage(Scripts.DialogSystem.ExternWord, dialogUnitInfo);
                                    }
                                    DestroyThis();
                                }));                              
                        }
                      
                    }

                };
                yield return new WaitForSeconds(.4f);
            }

            //如果有时间限制
            if (dialogUnitInfo.timeLimit)
            {
                var timer = 0f;
                MainLoop.Instance.UpdateForSeconds(() =>
                {
                    if (!m_TransFrom)
                        return;
                    onTimeLeave?.Invoke(timer, dialogUnitInfo.limitTime,defaultObj);
                    timer += Time.deltaTime;
                }, dialogUnitInfo.limitTime, () =>
                {
                    if (!m_TransFrom)
                        return;
                    //如果到现在没有点击下去
                    if (!clicked)
                    {
                        Hide();
                        MainLoop.Instance.StartCoroutine(
                            Scripts.DialogSystem.RunDialog(
                                dialogUnitInfo.Choices[dialogUnitInfo.defaultIndex].targetDialogInfoAsset,
                                () =>
                                {
                                    if (dialogUnitInfo.sendToExWord)
                                    {
                                        dialogUnitInfo.words = dialogUnitInfo.Choices[dialogUnitInfo.defaultIndex].text;
                                        CEventCenter.BroadMessage(Scripts.DialogSystem.ExternWord, dialogUnitInfo);
                                    }
                                    if (!dialogUnitInfo.Choices[dialogUnitInfo.defaultIndex].willBack)
                                        CEventCenter.BroadMessage(Scripts.DialogSystem.ChooseNotBack);
                                    DestroyThis();
                                }));
                    }
                });
            }

        }

    }
}