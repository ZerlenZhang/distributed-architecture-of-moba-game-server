using System;
using UnityEngine;
namespace ReadyGamerOne.Common
{
    /// <summary>
    /// MonoBehavior全局单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GlobalMonoSingleton<T> : MonoSingleton<T>
        where T : GlobalMonoSingleton<T>
    {
        protected override void OnStateIsNull()
        {
            base.OnStateIsNull();
            if (this.transform.parent)
            {
                throw new Exception($"全局单例物体必须是场景根物体:{name},parent【{transform.parent.name}】");
            }
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// 全局单例，如果重复，直接干掉
        /// </summary>
        protected override void OnStateIsOthers()
        {
            base.OnStateIsOthers();
            GameObject.DestroyImmediate(this.gameObject);
        }
    }
}
