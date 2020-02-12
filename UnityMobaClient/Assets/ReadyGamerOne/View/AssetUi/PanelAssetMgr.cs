using System;
using System.Collections.Generic;

namespace ReadyGamerOne.View.AssetUi
{
    public static class PanelAssetMgr
    {
        /// <summary>
        /// 全局清空ui的回调
        /// </summary>
        public static event Action onClear;

        /// <summary>
        /// 当前Ui资源
        /// </summary>
        public static PanelUiAsset CurrentPanelUi
        {
            get { return uiAssets.Count == 0 ? null : uiAssets.Peek(); }
        }

        /// <summary>
        /// 打开另一个Ui面板
        /// </summary>
        /// <param name="asset"></param>
        public static void PushPanel(PanelUiAsset asset)
        {
            if (uiAssets.Count != 0)
            {
                var a = uiAssets.Peek();
                a.Hide();
            }

            if (asset == null)
                return;
            
            asset.Show();
            uiAssets.Push(asset);
        }

        /// <summary>
        /// 关闭当前面板
        /// </summary>
        public static void PopPanel()
        {
            if (uiAssets.Count != 0)
                uiAssets.Pop().DestoryObj();
            if(uiAssets.Count!=0)
                uiAssets.Peek().Show();
        }

        /// <summary>
        /// 清空所有缓存的Ui资源
        /// </summary>
        public static void Clear()
        {
            while (uiAssets.Count>0)
            {
                PopPanel();
            }
            onClear?.Invoke();
        }
        


        private static Stack<PanelUiAsset> uiAssets = new Stack<PanelUiAsset>();
    }
}