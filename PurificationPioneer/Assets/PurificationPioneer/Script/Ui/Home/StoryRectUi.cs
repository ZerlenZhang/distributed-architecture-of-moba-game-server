using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class StoryRectUi : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public void Init(HeroConfigAsset config)
        {
            var index = 0;
            foreach (var story in config.stroies)
            {
                var storyUnitUi = ResourceMgr.InstantiateGameObject(UiName.StoryUnitUi, scrollRect.content)
                    .GetComponent<StoryUnitUi>();

                var storyIndex = ++index;
                var needTrustValue = 25 * storyIndex;
                storyUnitUi.Set($"档案--{storyIndex}", story, needTrustValue, config.trustValue < needTrustValue);
            }
        }
    }
}