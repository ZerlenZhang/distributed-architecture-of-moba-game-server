using System;
using System.Collections;

namespace ReadyGamerOne.MemorySystem
{
    public interface IResourceLoader
    {
        void Init(IHotUpdatePath pather, IOriginAssetBundleUtil originConstData,IAssetConstUtil assetConstUtil);
        T GetAsset<T>(string objKey, string bundleName = null)
            where T : UnityEngine.Object;
        IEnumerator GetAssetAsync<T>(string objName, string bundleKey = null, Action<T> onGetObj = null)
            where T:UnityEngine.Object;

        void ClearCache();
    }
}