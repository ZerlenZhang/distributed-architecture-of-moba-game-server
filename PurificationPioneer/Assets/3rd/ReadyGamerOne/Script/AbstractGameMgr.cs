using System;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.MemorySystem;
using UnityEngine.SceneManagement;

namespace ReadyGamerOne.Script
{
    /// <summary>
    /// 使用SceneMgr推荐用脚本继承这个类
    /// </summary>
    public abstract class AbstractGameMgr<T>:GlobalMonoSingleton<T>
        where T:AbstractGameMgr<T>
    {
        public static event Action onDrawGizomos;

        protected override void OnStateIsNull()
        {
            base.OnStateIsNull();
            print("AbstractGameMgr_Awake——这句话应该只显示一次");
            ResourceMgr.Init(ResourceLoader,PathData,OriginBundleData,AssetConstUtil);
            RegisterSceneEvent();
            QuickStartTool.RegisterUi(GetType());            
        }
        
        protected abstract IResourceLoader ResourceLoader { get; }
        protected virtual IAssetConstUtil AssetConstUtil => null;
        protected virtual IHotUpdatePath PathData => null;
        protected virtual IOriginAssetBundleUtil OriginBundleData => null;

        protected virtual void RegisterSceneEvent()
        {
            SceneManager.sceneLoaded +=(scene,mode)=> this.OnAnySceneLoad();
            SceneManager.sceneUnloaded +=(scene)=> this.OnAnySceneUnload();            
        }

        protected virtual void OnAnySceneLoad()
        {
            
        }

        protected virtual void OnAnySceneUnload()
        {
            MainLoop.Instance.Clear();
        }
        
        private void OnDrawGizmos()
        {
            onDrawGizomos?.Invoke();
        }
    }
}