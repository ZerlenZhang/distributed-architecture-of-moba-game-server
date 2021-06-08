using DialogSystem.Model;
using DialogSystem.Scripts;
using ReadyGamerOne.Script;
using UnityEngine.UI;

namespace DialogSystem.View
{
    public abstract class AbstractWordDialogUI:DialogUnitUI
    {
        private bool enableGoNext = false;
        protected abstract string SpeakerTextPath { get; }
        protected virtual string DialogWordTextPath => null;
        protected virtual string CaptionWordTextPath => null;
        

        private DialogData _data;
        public AbstractWordDialogUI(DialogUnitInfo unitInfo):base(unitInfo)
        {
            Text words = null;

            //判断显示类型，是字幕式，还是对话框
            switch (unitInfo.showType)
            {
                case ShowType.Caption:
                    Create(unitInfo.abstractAbstractDialogInfoAsset.CaptionWordUiKeys[unitInfo.wordPrefabIndex]);

                    m_TransFrom.Find(SpeakerTextPath).GetComponent<Text>().text = unitInfo.SpeakerName;
                    words = m_TransFrom.Find(CaptionWordTextPath).GetComponent<Text>();
                    
                    break;
                case ShowType.Dialog:            
                    Create(unitInfo.abstractAbstractDialogInfoAsset.DialogWordUiKeys[unitInfo.wordPrefabIndex]);
                                     
                    _data = m_TransFrom.GetComponent<DialogData>();
                    
                    m_TransFrom.position = unitInfo.fromNull
                        ? _data.GetSuitPos()
                        : _data.GetSuitPos(Scripts.DialogSystem.GetCharacterObj(unitInfo.SpeakerName).transform);
                    
                    
                    words = m_TransFrom.Find(DialogWordTextPath).GetComponent<Text>();
                    break;
            }

            words.text = unitInfo.words;

            Show();
            
            words.gameObject.AddComponent<TypeWriterEffect>().StartWriter(
                unitInfo.abstractAbstractDialogInfoAsset.wordFinishTime,
                ()=>enableGoNext=true,
                ()=>canGoNext,
                unitInfo.abstractAbstractDialogInfoAsset.affectedByTimeScale);
        }

        protected override void Update()
        {
            base.Update();

            if (m_dialogUnitInfo.showType == ShowType.Dialog && !m_dialogUnitInfo.fromNull)
            {
                m_TransFrom.position = m_dialogUnitInfo.fromNull
                    ? _data.GetSuitPos()
                    : _data.GetSuitPos(Scripts.DialogSystem.GetCharacterObj(m_dialogUnitInfo.SpeakerName).transform);
            }
            


            if (enableGoNext && canGoNext)
                DestroyThis();
        }
    }
}