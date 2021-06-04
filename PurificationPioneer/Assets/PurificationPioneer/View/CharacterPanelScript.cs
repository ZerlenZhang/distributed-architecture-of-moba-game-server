using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class CharacterPanelScript : MonoBehaviour
    {
        public PropertyFieldsUi propertyFieldsUi;
        public HeroNameFieldUi heroNameFieldUi;
        public StoryRectUi storyRectUi;

        public Image mainPicture;


        public void Init(HeroConfigAsset config)
        {
            propertyFieldsUi.Init(config);
            heroNameFieldUi.Init(config);
            storyRectUi.Init(config);

            mainPicture.sprite = config.icon;
        }


        public void Return()
        {
            PanelMgr.PopPanel();
        }
    }
}