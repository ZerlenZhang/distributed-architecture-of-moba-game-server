using System;
using System.Collections;
using System.Collections.Generic;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace ReadyGamerOne.MemorySystem
{
    public class ResourcesResourceLoader:
        Singleton<ResourcesResourceLoader>,
        IResourceLoader
    {
        #region Fields

        private Dictionary<string, Object> sourceObjectDic;
        private IAssetConstUtil _assetConstUtil;

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
        
        #region IResourceLoader

        public void Init(IHotUpdatePath pather, IOriginAssetBundleUtil originConstData,IAssetConstUtil assetConstUtil)
        {
//            Debug.Log(assetConstUtil.GetType().FullName);
            Assert.IsNotNull(assetConstUtil);
            Assert.IsNotNull(assetConstUtil.NameToPath);
            this._assetConstUtil = assetConstUtil;
        }

        public T GetAsset<T>(string objName, string bundleKey = null) where T : Object
        {
            Assert.IsFalse(string.IsNullOrEmpty(objName));
            if (_assetConstUtil == null)
                throw new Exception("_assetConstUtil为空，没有初始化？？");
            if (_assetConstUtil.NameToPath == null)
            {
                throw new Exception("字典为空？？？");
            }
            if (!_assetConstUtil.NameToPath.ContainsKey(objName))
            {
                throw new Exception("没有这个Key ："+objName);
            }
            return GetSourceFromCache(_assetConstUtil.NameToPath[objName], Resources.Load<T>);
        }

        public IEnumerator GetAssetAsync<T>(string objName, string bundleKey = null, Action<T> onGetObj = null) where T : Object
        {
            Assert.IsNotNull(onGetObj);

            //如果缓存中有，就直接使用缓存
            var temp = GetSourceFromCache<T>(objName);
            if (null != temp)
            {
                onGetObj(temp);
                yield break;
            }
            
            if (!_assetConstUtil.NameToPath.ContainsKey(objName))
            {
                throw new Exception("没有这个objectKey ："+objName);
            }

            
            var request = Resources.LoadAsync<T>(_assetConstUtil.NameToPath[objName]);
            yield return request;

            var asset = (T) request.asset;

            if (!asset)
                throw new Exception("获取资源为空，objName: " + objName);

            SourceObjects.Add(objName, asset);
            onGetObj(asset);
        }

        public void ClearCache()
        {
            SourceObjects.Clear();
        }

        #endregion
    }
}