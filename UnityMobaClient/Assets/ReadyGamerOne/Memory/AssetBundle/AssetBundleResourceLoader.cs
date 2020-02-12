using System;
using System.Collections;
using System.Collections.Generic;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace ReadyGamerOne.MemorySystem
{
    public class AssetBundleResourceLoader:
        Singleton<AssetBundleResourceLoader>,
        IResourceLoader
    {
        #region Fields

        private Dictionary<string, Object> sourceObjectDic;
        private AssetBundleLoader assetBundleLoader;
        private IHotUpdatePath pather;
        private IOriginAssetBundleUtil originBundleConst;        

        #endregion

        #region Properties

        private Dictionary<string, Object> SourceObjects
        {
            get
            {
                if (sourceObjectDic == null)
                    sourceObjectDic = new Dictionary<string, Object>();

                return sourceObjectDic;
            }
            set { sourceObjectDic = value; }
        }        

        #endregion

        #region Private

        /// <summary>
        /// 从缓存中获取资源，否则采用默认方式初始化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultGetMethod"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private T GetSourceFromCache<T>(string key, Func<string, T> defaultGetMethod = null)
            where T : Object
        {
            if (SourceObjects.ContainsKey(key))
            {
                if (SourceObjects[key] == null)
                    throw new Exception("资源已经释放，但字典中引用亦然保留");

                var ans = SourceObjects[key] as T;
                if (ans == null)
                    Debug.LogWarning("资源引用存在，但类型转化失败，小心bug");

                return ans;
            }

            if (null == defaultGetMethod)
            {
                Debug.LogWarning("defaultGetMethod is null");
                return null;
            }

            var source = defaultGetMethod(key);
            if (source == null)
            {
                Debug.LogError("资源加载错误，错误路径：" + key);
                return null;
            }

            SourceObjects.Add(key, source);
            return source;
        }        

        #endregion
        
        public void ShowDebugInfo()
        {
            Debug.Log("《AssetBundle加载情况》\n" + assetBundleLoader.DebugInfo());
        }

        #region IResourceLoader
        
        public void Init(IHotUpdatePath pather, IOriginAssetBundleUtil originConstData,IAssetConstUtil assetConstUtil)
        {
            if (null == pather)
                return;
            if (null == originConstData)
                return;
            this.originBundleConst = originConstData;
            this.pather = pather;
            assetBundleLoader = new AssetBundleLoader();
            MainLoop.Instance.StartCoroutine(assetBundleLoader.StartBundleManager(pather, originConstData));
        }

        public T GetAsset<T>(string objName, string bundleKey = null)   
            where T : Object
        {
            if (null == pather)
                throw new Exception("没有初始化MemoryMgr.Pather");

            if (originBundleConst == null)
                throw new Exception("originBundleConst is null");


            return GetSourceFromCache(objName,
                key =>
                {
                    var ab = assetBundleLoader.GetBundle(bundleKey);
                    Assert.IsTrue(ab);
                    return ab.LoadAsset<T>(key);
                });
        }

        public IEnumerator GetAssetAsync<T>(string objName, string bundleKey = null, Action<T> onGetObj = null)
            where T : Object
        {
            if (null == pather)
                throw new Exception("没有初始化MemoryMgr.Pather");

            Assert.IsNotNull(onGetObj);

            if (originBundleConst.KeyToName.ContainsKey(bundleKey))
            {    
                var bundleName = originBundleConst.KeyToName[bundleKey];
             
                //如果缓存中有，就直接使用缓存
                var temp = GetSourceFromCache<T>($"{bundleName}_{objName}");
                if (null != temp)
                {
                    onGetObj(temp);
                    yield break;
                }
            }
            

            //缓存没有，使用加载器加载
            yield return assetBundleLoader.GetBundleAsync(bundleKey,
                ab => GetAssetFormBundleAsync(ab, objName, onGetObj));



        }
        private IEnumerator GetAssetFormBundleAsync<T>(AssetBundle assetBundle, string _objName, Action<T> _onGetObj)
            where T : Object
        {
            var objRequest = assetBundle.LoadAssetAsync<T>(_objName);
            yield return objRequest;

            //添加到缓存
            var ans = objRequest.asset as T;
            if (null == ans)
                throw new Exception("Get Asset is null");
            SourceObjects.Add($"{assetBundle.name}_{_objName}", ans);

            var asset = (T) objRequest.asset;
            Assert.IsNotNull(asset);
            //使用物品
            _onGetObj(asset);
        }
        public void ClearCache()
        {
            SourceObjects.Clear();
        }

        #endregion
    }
}