using System.Collections.Generic;

namespace ReadyGamerOne.Rougelike.Item
{
    /// <summary>
    /// 具有获取道具功能的角色需要实现此接口
    /// </summary>
    /// <typeparam name="ItemType"></typeparam>
    public interface IItemablePerson<ItemType>
        where ItemType:IItemInfo
    {
        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="itemId">添加物品的Id</param>
        /// <param name="count">添加物品个数</param>
        /// <returns>当前这个物品的信息</returns>
        ItemType AddItem(string itemId,int count=1);
        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemId">移除物品个数</param>
        /// <param name="count">移除物品个数</param>
        /// <returns>当前这个物品的信息</returns>
        ItemType RemoveItem(string itemId,int count=1);
        /// <summary>
        /// 获取物品信息
        /// </summary>
        /// <returns></returns>
        Dictionary<string,ItemType> GetItems();
        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        int GetItemCount(string itemId);
    }
}