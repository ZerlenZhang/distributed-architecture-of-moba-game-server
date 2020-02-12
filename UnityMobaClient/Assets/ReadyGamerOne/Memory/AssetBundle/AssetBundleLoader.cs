using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace ReadyGamerOne.MemorySystem
{
    /// <summary>
    /// AB包加载器，管控已经位于本地的AB包在游戏中的加载和卸载
    /// 管理载入情况，管理依赖，管理引用次数
    /// </summary>
    internal class AssetBundleLoader
    {
        private Dictionary<string, BundleUsageInfo> HeldedBundles = new Dictionary<string, BundleUsageInfo>();

        private AssetBundleLoaderTool loadLoaderTool;
        private AssetBundleDownLoader assetBundleDownLoader;
        private ManifestReader manifestReader;
        private IOriginAssetBundleUtil _originAssetBundleUtil;

        /// <summary>
        /// 获取调试信息
        /// </summary>
        /// <returns></returns>
        public string DebugInfo()
        {
            var ans = "";
            foreach (var kv in HeldedBundles)
            {
                ans += kv.Key + " " + kv.Value.useTime + "\n";
            }

            return ans;
        }
        
        #region 资源管控

        /// <summary>
        /// 开启整个工作流程
        /// </summary>
        /// <returns></returns>
        internal IEnumerator StartBundleManager(IHotUpdatePath pather, IOriginAssetBundleUtil originAssetBundleUtil = null)
        {
            this._originAssetBundleUtil = originAssetBundleUtil;

            manifestReader = new ManifestReader(pather);
            yield return manifestReader.LoadManifest();

            if (!string.IsNullOrEmpty(pather.WebServeMainManifest))
                assetBundleDownLoader = new AssetBundleDownLoader(pather);

            loadLoaderTool = new AssetBundleLoaderTool(OnAddAssetBundle, assetBundleDownLoader, pather, originAssetBundleUtil);

            if (!string.IsNullOrEmpty(pather.WebServeMainManifest))
                yield return assetBundleDownLoader.CheckVersionAndUpdate();
        }

        /// <summary>
        /// 异步获取AB包并使用，如果没有加载AB包就异步加载，顺利调用会使对应AB包引用计数加一
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="objectName"></param>
        /// <param name="getAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal IEnumerator GetBundleAsync(string bundleName, System.Func<AssetBundle, IEnumerator> getAction)
        {
            //首次加载时，需要先加载依赖
            yield return AddBundleWithDependenciesAsync(bundleName);

            Assert.IsTrue(HeldedBundles.ContainsKey(bundleName));

            var targetBundle = HeldedBundles[bundleName].heldAssetBundle;
            HeldedBundles[bundleName].useTime++;
            yield return getAction(targetBundle);


        }
        private IEnumerator AddBundleWithDependenciesAsync(string _bundleName)
        {
            //如果已经加载过，就无需在加载
            if (HeldedBundles.ContainsKey(_bundleName))
            {
                yield break;
            }

            //获取依赖
            var dependences = manifestReader.GetBundleDependencies(_bundleName);

            //如果没有依赖，直接加载自己，然后返回
            if (null == dependences)
            {
                yield return loadLoaderTool.LoadFileAsync(_bundleName);
                yield break;
            }

            //如果有依赖，逐个添加依赖
            foreach (var dependence in dependences)
            {
                //添加依赖，保证已经加载这个依赖
                yield return AddBundleWithDependenciesAsync(dependence);

                //添加一次依赖
                HeldedBundles[dependence].useTime++;
            }

            //如果有依赖，添加完依赖后，最后添加自己，这样，无论有没有以来，最后都会连自己都添加了
            yield return loadLoaderTool.LoadFileAsync(_bundleName);
        }
        
        
        /// <summary>
        /// 同步加载一个没有依赖的AB包，优先从缓存获取，顺利调用会使对应AB包引用计数加一
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="defaultGetMethod"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal AssetBundle GetBundle(string bundleNameKey)
        {
            if (string.IsNullOrEmpty(bundleNameKey))
            {
                throw new Exception("Ab包Key不得为空");
            }
            Assert.IsNotNull(_originAssetBundleUtil);
            
            var bundleName = _originAssetBundleUtil.KeyToName[bundleNameKey];
            
            //待依赖添加资源
            AddBundleWithDependencies(bundleName,_originAssetBundleUtil);

            Assert.IsTrue(HeldedBundles.ContainsKey(bundleName));

            var targetBundle = HeldedBundles[bundleName].heldAssetBundle;
            HeldedBundles[bundleName].useTime++;

            return targetBundle;
        }

        #region Private

        /// <summary>
        /// 每次加载一个AssetBundle的回调
        /// </summary>
        /// <param name="bundle"></param>
        void OnAddAssetBundle(AssetBundle bundle)
        {
//            var assets = bundle.LoadAllAssets();
//            foreach (var asset in assets)
//            {
//                Debug.Log(asset.ToString());
//            }

            if (!HeldedBundles.ContainsKey(bundle.name))
            {
//                Debug.Log("Add  " + bundle.name);
                HeldedBundles.Add(bundle.name, new BundleUsageInfo(bundle));
            }
        }

        #endregion

        /// <summary>
        /// 同步添加依赖
        /// </summary>
        /// <param name="_bundleName"></param>
        void AddBundleWithDependencies(string _bundleName,IOriginAssetBundleUtil originAssetBundleUtil)
        {
            //如果已经加载过，就无需在加载
            if (HeldedBundles.ContainsKey(_bundleName))
            {
                return;
            }

            //获取依赖
            var dependences = manifestReader.GetBundleDependencies(_bundleName);

            //如果没有依赖，直接加载自己，然后返回
            if (null == dependences)
            {
                loadLoaderTool.LoadFile(originAssetBundleUtil.NameToPath[_bundleName]);
                return;
            }

            //如果有依赖，逐个添加依赖
            foreach (var dependence in dependences)
            {
                //添加依赖，保证已经加载这个依赖
                AddBundleWithDependencies(dependence,originAssetBundleUtil);

                //添加一次依赖
                HeldedBundles[dependence].useTime++;
            }

            //如果有依赖，添加完依赖后，最后添加自己，这样，无论有没有以来，最后都会连自己都添加了
            loadLoaderTool.LoadFile(originAssetBundleUtil.NameToPath[_bundleName]);
        }

        #endregion

        #region 内部类

        /// <summary>
        /// 记录AB包间相互依赖的数据结构
        /// </summary>
        private class BundleUsageInfo
        {
            public AssetBundle heldAssetBundle;
            public int useTime;

            public BundleUsageInfo(AssetBundle inbundle)
            {
                heldAssetBundle = inbundle;
            }
        }

        /// <summary>
        /// 读取资源包，提供多种方式
        /// 1.读取本地，并且使用
        /// 2.下载并且使用web服务器提供的
        /// 3.按字节读取本地或者web的，然后自己来解密
        /// </summary>
        private class AssetBundleLoaderTool
        {
            internal AssetBundleLoaderTool(Action<AssetBundle> onLoadEveryComplete, AssetBundleDownLoader version,
                IHotUpdatePath updatePath, IOriginAssetBundleUtil originAssetBundleUtil)
            {
                this.pather = updatePath;
                this.onLoadEveryComplete = onLoadEveryComplete;
                this.version = version;
                this._originAssetBundleUtil = originAssetBundleUtil;
            }

            private Action<AssetBundle> onLoadEveryComplete;
            private AssetBundleDownLoader version;
            private IHotUpdatePath pather;
            private IOriginAssetBundleUtil _originAssetBundleUtil;

            private class BundleContainer
            {
                public AssetBundle assetBundle;
                public byte[] bytes;
            }

            private enum BundleLoadMethod
            {
                WWW_LoadBundle = 0,
                WWW_LoadBytes = 1,
                WWW_LoadBundleAndBytes = 2,
                File_LocalBundle = 3,
                File_Bytes = 4,
                File_StreamingBundle = 5
            };

            /// <summary>
            /// 留给外部的接口，根据bundle名异步加载文件，回调需要提前赋值
            /// </summary>
            /// <param name="bundleName"></param>
            /// <returns></returns>
            internal IEnumerator LoadFileAsync(string bundleName)
            {
                //是否使用StreamingAssets?
                if (!string.IsNullOrEmpty(pather.OriginMainManifest))
                {
                    var streamingPath = @Path.Combine(Application.streamingAssetsPath, bundleName);
                    if (File.Exists(streamingPath))
                    {
//                        Debug.Log("LoadStreaming");
                        yield return LoadBundleAsync(bundleName, null, BundleLoadMethod.File_StreamingBundle);
                        yield break;
                    }
                }

                //是否使用服务器资源
                if (!string.IsNullOrEmpty(pather.WebServeMainManifest))
                {
                    Assert.IsNotNull(version);

                    while (!version.IsUpdateToServe)
                        yield return null;
                    var bundleVersion = version.currentServeVersion;


                    var localBundlePath = pather.GetLocalBundlePath(bundleName, bundleVersion);

                    if (File.Exists(localBundlePath))
                    {
//                        Debug.Log("LoadLocalFile");
                        yield return LoadBundleAsync(bundleName, bundleVersion, BundleLoadMethod.File_LocalBundle);
                        yield break;
                    }

//                    Debug.Log("LoadWWW");
                    yield return LoadBundleAsync(bundleName, bundleVersion, BundleLoadMethod.WWW_LoadBundleAndBytes,
                        true);
                    yield break;
                }

                Debug.LogError("没有这个AssetBundle: " + bundleName);
            }

            internal void LoadFile(string bundlePath)
            {
                Assert.IsNotNull(_originAssetBundleUtil);
                try
                {
                    var ab = AssetBundle.LoadFromFile(bundlePath);
                    onLoadEveryComplete(ab);
                }
                catch (KeyNotFoundException)
                {
                    throw new Exception("没有这个BundleName " + bundlePath);
                }

            }

            /// <summary>
            /// 以字节流的形式载入资源
            /// </summary>
            private IEnumerator LoadBundleAsync(string bundleName, string bundleVersion, BundleLoadMethod method,
                bool saveBundle = false)
            {
                string bundlePath;
                var container = new BundleContainer();
                switch (method)
                {
                    case BundleLoadMethod.File_StreamingBundle:
                        bundlePath = @Path.Combine(Application.streamingAssetsPath, bundleName);
                        break;
                    case BundleLoadMethod.File_LocalBundle:
                        bundlePath = pather.GetLocalBundlePath(bundleName, bundleVersion);
                        break;
                    default:
                        bundlePath = pather.GetServeBundlePath(bundleName, bundleVersion);
                        break;
                }

                yield return ExecuteLoadBundleAsync(bundlePath, method, container);

                onLoadEveryComplete(container.assetBundle);

                if (saveBundle && container.bytes != null)
                {
                    var filePath = pather.GetLocalBundlePath(bundleName, bundleVersion);
                    FileUtil.CreateFile(filePath, container.bytes);
                }
            }

            /// <summary>
            /// 读取字节
            /// 1.File.ReadAllBytes(path)
            /// 2.WWW
            /// </summary>
            /// <param name="path">路径</param>
            /// <param name="streamMethod">形式</param>
            /// <param name="cotiner">字节容器</param>
            /// <returns></returns>
            private IEnumerator ExecuteLoadBundleAsync(string path, BundleLoadMethod streamMethod,
                BundleContainer container)
            {
//                Debug.Log(streamMethod);
                switch (streamMethod)
                {
                    case BundleLoadMethod.WWW_LoadBundleAndBytes:
                    {
                        var webBbLoadRequest = UnityWebRequest.Get(path);
                        yield return webBbLoadRequest.SendWebRequest();
                        container.bytes = webBbLoadRequest.downloadHandler.data;
                        var bundleCreationRequest = AssetBundle.LoadFromMemoryAsync(container.bytes);
                        yield return bundleCreationRequest;
                        container.assetBundle = bundleCreationRequest.assetBundle;
                        break;
                    }

                    case BundleLoadMethod.WWW_LoadBytes:
                    {
                        var webBytesLoadRequest = UnityWebRequest.Get(path);
                        yield return webBytesLoadRequest.SendWebRequest();
                        container.bytes = webBytesLoadRequest.downloadHandler.data;
                        break;
                    }

                    case BundleLoadMethod.WWW_LoadBundle:
                    {
                        var webBundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(path);
                        yield return webBundleRequest.SendWebRequest();
                        container.assetBundle = DownloadHandlerAssetBundle.GetContent(webBundleRequest);
                        break;
                    }

                    case BundleLoadMethod.File_LocalBundle:
                    {
                        var fileBundleRequest = AssetBundle.LoadFromFileAsync(path);
                        yield return fileBundleRequest;
                        container.assetBundle = fileBundleRequest.assetBundle;
                        break;
                    }

                    case BundleLoadMethod.File_StreamingBundle:
                    {
                        var fileBundleRequest = AssetBundle.LoadFromFileAsync(path);
                        yield return fileBundleRequest;
                        container.assetBundle = fileBundleRequest.assetBundle;
                        break;
                    }

                    case BundleLoadMethod.File_Bytes:
                        break;
                    default:
                        yield break;
                }
            }
        }

        /// <summary>
        /// 读取Manifest并且解析依赖关系
        /// </summary>
        private class ManifestReader
        {
            /// <summary>
            /// 记录各AB包依赖依赖
            /// </summary>
            private Dictionary<string, string[]> bundleDependencies = new Dictionary<string, string[]>();

            private IHotUpdatePath pather;

            internal ManifestReader(IHotUpdatePath updatePath)
            {
                this.pather = updatePath;
            }

            /// <summary>
            /// 从服务器获取Manifest，记录依赖
            /// </summary>
            /// <returns></returns>
            internal IEnumerator LoadManifest()
            {
                //如果使用服务器资源
                if (!string.IsNullOrEmpty(pather.WebServeMainManifest))
                {
                    var webBundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(pather.WebServeMainManifest);
                    yield return webBundleRequest.SendWebRequest();

                    var serverAssetBundle = DownloadHandlerAssetBundle.GetContent(webBundleRequest);
                    if (!serverAssetBundle)
                        throw new Exception("server assetBundle is null");

                    var assetR = serverAssetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                    yield return assetR;
                    var manifest = assetR.asset as AssetBundleManifest;

                    Assert.IsTrue(manifest);
                    foreach (var bundlesName in manifest.GetAllAssetBundles())
                    {
                        bundleDependencies.Add(bundlesName, manifest.GetAllDependencies(bundlesName));
                    }

                    serverAssetBundle.Unload(true);
                }

                //如果使用StreamingAsset资源
                if (!string.IsNullOrEmpty(pather.OriginMainManifest))
                {
                    var originBundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(pather.OriginMainManifest);
                    yield return originBundleRequest.SendWebRequest();

                    var originAssetBundle = DownloadHandlerAssetBundle.GetContent(originBundleRequest);
                    if (!originAssetBundle)
                        throw new Exception("Streaming AssetBundle is null");

                    var assetR = originAssetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                    yield return assetR;
                    var manifest = assetR.asset as AssetBundleManifest;

                    Assert.IsTrue(manifest);
                    foreach (var bundlesName in manifest.GetAllAssetBundles())
                    {
                        bundleDependencies.Add(bundlesName, manifest.GetAllDependencies(bundlesName));
                    }

                    originAssetBundle.Unload(true);
                }
            }

            /// <summary>
            /// 获取bundle依赖
            /// </summary>
            /// <param name="bundleName"></param>
            /// <returns></returns>
            internal string[] GetBundleDependencies(string bundleName)
            {
                if (!bundleDependencies.ContainsKey(bundleName))
                    return null;
                if (bundleDependencies[bundleName].Length == 0)
                    return null;
                else
                    return bundleDependencies[bundleName];
            }
        }

        #endregion
    }
}