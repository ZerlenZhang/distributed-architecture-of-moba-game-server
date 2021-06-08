using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyGamerOne.View
{
    [Serializable]
    public class TabPair
    {
        public bool hasInit = false;
        public int index;
        public Button tab;
        public GameObject page;
    }

    public class TabBar : UnityEngine.MonoBehaviour
    {
        private void OnEnable()
        {
            var temp = new List<TabPair>();
            foreach (var tabPair in tabPairs)
            {
                if (tabPair.page == null || tabPair.tab == null)
                    temp.Add(tabPair);
            }
            foreach (var tabPair in temp)
            {
                Debug.LogWarning("清除无效tabPair");
                tabPairs.Remove(tabPair);
            }
            var index = 0;
            foreach (var tabPair in tabPairs)
            {
                if (tabPair.hasInit)
                    return;
                tabPair.tab.onClick.AddListener(
                    () => OnClickTab(tabPair.index));
                tabPair.hasInit = true;
                SetTabState(index, index == 0);
                index++;
            }
        }

        /// <summary>
        /// 点击tab时触发
        /// <GameObject>点击的物体</GameObject>
        /// <int>点击物体的索引</int>
        /// </summary>
        public event Action<GameObject, int> onSelectChanged;

        /// <summary>
        /// 如何设置某个选项物体的开启与关闭状态，默认使用SetActive
        /// </summary>
        public Action<GameObject, bool> howSetTabState;


        /// <summary>
        /// 当前选择的选项卡索引
        /// </summary>
        public int SelectedIndex => _selectIndex;

        /// <summary>
        /// 当前选择的选项卡物体
        /// </summary>
        public GameObject SelectObj
        {
            get
            {
                if (tabPairs.Count==0)
                    return null;
                return tabPairs[_selectIndex].page;
            }
        }

        [SerializeField]
        private int _selectIndex = -1;
        

        /// <summary>
        /// 默认采用设置物体Active的方法控制激活与否
        /// </summary>
        /// <param name="page"></param>
        /// <param name="state"></param>
        private Action<int, bool> SetTabState =>
            (index, state) =>
            {
                var func = howSetTabState ?? DefaultSetTabState;
                func.Invoke(tabPairs[index].page, state);
                if (state)
                {
                    _selectIndex = index;
                    onSelectChanged?.Invoke(tabPairs[_selectIndex].page, _selectIndex);
                }
            };

        private void DefaultSetTabState(GameObject page, bool state)
        {
            if (null == page)
            {
                Debug.LogWarning("选项卡物体为空");
                return;
            }
            page.SetActive(state);
        }

        [SerializeField] private List<TabPair> tabPairs = new List<TabPair>();

        
        /// <summary>
        /// 添加TabPair，第三个参数用于编辑器，代码调用不能填
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="page"></param>
        /// <param name="index"></param>
        public void AddTabPair(Button btn, GameObject page, int index = -1)
        {
            //遍历列表，避免重复
            foreach (var tabPair in tabPairs)
            {
                if (tabPair.tab != btn && tabPair.page != page) continue;
                Debug.LogWarning("重复加入tab或page");
                return;
            }

            var pair = new TabPair
            {
                hasInit = true,
                index = index == -1 ? tabPairs.Count : index,
                tab = btn,
                page = page,
            };

            //按钮添加监听事件
            btn.onClick.AddListener(
                () => OnClickTab(pair.index));
            tabPairs.Add(pair);

            //如果是第一个选项卡，默认选中
            if (pair.index == 0)
            {
                SetTabState(0, true);
            }
            else
            {
                SetTabState(pair.index, false);
            }
        }
        
        private void OnClickTab(int index)
        {
            if (_selectIndex == index)
                return;

            SetTabState(_selectIndex, false);
            SetTabState(index, true);
        }

    }
}
     