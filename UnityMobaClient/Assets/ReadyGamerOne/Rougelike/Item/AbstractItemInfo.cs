using ReadyGamerOne.Common;

namespace ReadyGamerOne.Rougelike.Item
{
    public interface IItemInfo : IHasStringId
    {
        string ItemName { get; }
        string UiText { get; }
        int Count { get; }
        string SpriteName { get; }
        string ItemPrefabName { get; }
        string Statement { get; }
        int MaxSlotCount { get; }
    }
    
    public abstract class AbstractItemInfo:
        IItemInfo
    {
        public abstract string ID { get; }
        
        /// <summary>
        /// 物品名，需要符合变量命名规则，同时和Sprite资源名一致
        /// </summary>
        public abstract string ItemName { get; }
        
        /// <summary>
        /// 显示给玩家看的名字，一般是中文
        /// </summary>
        public abstract string UiText { get; }
        
        /// <summary>
        /// 当前拥有的物品数量
        /// </summary>
        public abstract int Count { get; set; }
        
        /// <summary>
        /// 图片资源加载路径
        /// </summary>
        public abstract string SpriteName { get; }

        /// <summary>
        /// 物品预制体加载路径
        /// </summary>
        public virtual string ItemPrefabName => null;
        
        /// <summary>
        /// 物品描述
        /// </summary>
        public virtual string Statement => "没有物品描述";
        
        /// <summary>
        /// 一个格子最多放置多少该物品
        /// </summary>
        public virtual int MaxSlotCount => 999;
    }
}