using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ReadyGamerOne.Const;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace ReadyGamerOne.MemorySystem
{
    /// <summary>
    /// AB包下载器，负责检查本地和服务器资源结构的区别，并把新资源下载到本机
    /// 并且维护不同的资源
    /// </summary>
    internal class AssetBundleDownLoader
    {
        #region 流程

        //读取服务器端和版本相关的资源配置文档,非manifest
        //配置文档包括当前版本,资源文件对应的版本如
        //CurrentVersion:0.12
        //SomeBundleVersion:0.10
        //检查本地的配置。找到不同之后，重新下载
        //下载完成后，将最新版本的配置写入本地

        #endregion

        internal string currentServeVersion;

        public bool IsUpdateToServe { get; private set; }

        internal AssetBundleDownLoader(IHotUpdatePath pather)
        {
            this.pather = pather;
            Assert.IsFalse(string.IsNullOrEmpty(pather.WebServeMainManifest));
            bundleDownloaderTool = new BundleDownloaderTool(OnDownloadCompleteF, OnAllResourceFine, pather);
        }
       
        /// <summary>
        /// 检查本地和服务器资源结构并更新
        /// </summary>
        /// <returns></returns>
        internal IEnumerator CheckVersionAndUpdate()
        {
            //读取当前客户端资源结构
            ReadLocalCurentVersionData();
            //读取当前服务器资源结构
            yield return ReadServeNewestVersion();
            //比较服务器和当前版本差别
            var differenceConfig = CheckDifference();

            Debug.Log("区别：" + differenceConfig);
            //如果版本检查失败（网络链接异常什么的)
            if (VersionCheckFailed)
            {
                Debug.Log("版本检查失败");
                IsUpdateToServe = false;
                //直接结束
                yield break;
            }
            
            
            //如果没有区别
            if (differenceConfig == null)
            {
                Debug.Log("没有区别");
                //设置同步完成
                currentLocalVersion = currentServeVersion;
                IsUpdateToServe = true;
            }
            else
            {
                Debug.Log("有区别");
                //如果有区别，就要下载有区别的文件
                if (differenceConfig.Config.Count != 0)
                {
                    DownLoadAllDifferenceResource(differenceConfig);
                }
            }
        }

        #region Private

        private string currentLocalVersion;
        private BundleDownloaderTool bundleDownloaderTool;
        private ResourcesConfig serveConfig;
        private ResourcesConfig localConfig;
        private bool VersionCheckFailed = false;
        private IHotUpdatePath pather;
        
        /// <summary>
        /// 读取服务器版本信息
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadServeNewestVersion()
        {
       
            
            string versionMessage = null;
            yield return FetchMessageFromWeb(pather.WebServeVersionPath, output => versionMessage = output);
            if (!string.IsNullOrEmpty(versionMessage))
            {
                //记录当前服务器版本号
                currentServeVersion = versionMessage;


                var serveConfigURL = pather.GetServeConfigPath(versionMessage);
                //Debug.Log(NewestServeConfigURL);
                string serverData = null;
                yield return FetchMessageFromWeb(serveConfigURL, output => serverData = output);
                //Debug.Log(servedata);
                //记录当前服务器资源结构
                serveConfig = ResourcesConfig.Create(serverData);
            }
        }
        
        /// <summary>
        /// 从服务器获取信息
        /// </summary>
        /// <param name="webPath">从哪个网页获取信息</param>
        /// <param name="outMessage">如何操作获取的信息</param>
        /// <returns></returns>
        private IEnumerator FetchMessageFromWeb(string webPath, System.Action<string> outMessage)
        {
            using (var webRequest = UnityWebRequest.Get(webPath))
            {
                yield return webRequest.SendWebRequest();
                var text = webRequest.downloadHandler.text;
                outMessage(text);
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    VersionCheckFailed = true;
                    Debug.LogWarning(webRequest.error);
                }
            }
        }          
        
        
        /// <summary>
        /// 读取当前本地客户端版本
        /// </summary>
        private void ReadLocalCurentVersionData()
        {
            //记录客户端版本号
            var newestVersion = PlayerPrefs.GetString(VersionDefine.PrefKey_LocalVersion);
            var versionData = PlayerPrefs.GetString(newestVersion);
            //记录客户端版本资源结构
            localConfig = ResourcesConfig.Create(versionData);
        }
        
        /// <summary>
        /// 比较服务器和本地版本的不同
        /// </summary>
        /// <returns></returns>
        private ResourcesConfig CheckDifference()
        {
            if (serveConfig.Config == null)
            {
                VersionCheckFailed = true;
                return null;
            }

            return serveConfig.CheckDifference(localConfig);
        }

        /// <summary>
        /// 下载不同的素材到不同的文件夹中
        /// </summary>
        /// <param name="differenceConfig"></param>
        private void DownLoadAllDifferenceResource(ResourcesConfig differenceConfig)
        {
            if (differenceConfig == null)
                return;
            //把所有不同的素材添加到下载队列中
            foreach (var item in differenceConfig.Config)
            {
                bundleDownloaderTool.AddDownloadBundleCommand(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 下载完成时回调
        /// </summary>
        private void OnDownloadCompleteF()
        {
            bundleDownloaderTool.CheckFileDone(serveConfig);
        }

        /// <summary>
        /// 更新结束后回调
        /// </summary>
        private void OnAllResourceFine()
        {
            //设定更新完成
            IsUpdateToServe = true;
            //更新本地
            currentLocalVersion = currentServeVersion;
            PlayerPrefs.SetString(VersionDefine.PrefKey_LocalVersion, currentLocalVersion);
            PlayerPrefs.SetString(currentLocalVersion, serveConfig.ToString());
            Debug.Log("所有资源更新已就位");
        }
        
        #endregion

        
        #region 内部类

        /// <summary>
        /// Resources信息结构体，用于标识某一个AssetBundle及其版本
        /// </summary>
        [System.Serializable]
        private struct VersionedResource
        {
            /// <summary>
            /// AssetBundle名
            /// </summary>
            internal string BundleName;
            /// <summary>
            /// AB包版本，一般是小数
            /// </summary>
            internal string Version;
        }
        
        /// <summary>
        /// 资源结构类，标识大量资源，可以标识本地资源结构树，服务器资源结构树
        /// </summary>
        private class ResourcesConfig
        {
            #region 对外唯一创建函数

            public static ResourcesConfig Create(string inputMessage)
            {
                var ans = new ResourcesConfig();
                
                if (string.IsNullOrEmpty(inputMessage))
                    return ans;
                
                inputMessage = inputMessage.Replace("\r", string.Empty);
                var lines = inputMessage.Split('\n');
                if (lines.Length == 0)
                    return null;
                foreach (var line in lines)
                {
                    var sets = line.Split(':');
                    ans.Config.Add(sets[0], sets[1]);               
                }
                return ans;
            }        

            #endregion

            #region Private

            private Dictionary<string, string> config;        
            private ResourcesConfig()
            {
                
            }
            #endregion


            internal Dictionary<string, string> Config 
            {
                get
                {
                    if (null == config)
                        config = new Dictionary<string, string>();
                    return config;
                }
            }

            
            /// <summary>
            /// 将字典信息用"\n"拼成字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                var sb = new System.Text.StringBuilder();
                int c = Config.Count;
                foreach (var item in Config)
                {
                    c--;
                    if (c > 0)
                    {
                        sb.AppendFormat("{0}:{1}\n", item.Key, item.Value);
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", item.Key, item.Value);
                    }
                }

                return sb.ToString();
            }
            
            /// <summary>
            /// 对比两个的不同，以自身为主
            /// </summary>
            /// <param name="inputConfig"></param>
            /// <returns></returns>
            internal ResourcesConfig CheckDifference(ResourcesConfig inputConfig)
            {
                ResourcesConfig differenceConfig = new ResourcesConfig();
                
                foreach (var item in Config)
                {
                    //对于每一个自己的item，如果对方也有
                    if (inputConfig.Config.ContainsKey(item.Key))
                    {
                        //判断版本是否相同
                        if (inputConfig.Config[item.Key] != item.Value)
                        {
                            // 如果版本不同就添加自身的数据到结果中，
                            // 这一行代码说明，这样的对比是默认采用本类数据的，所以，
                            // 调用的时候，应该是 服务器数据结构.CheckDifference(客户端数据结构）
                            differenceConfig.Config.Add(item.Key, item.Value);
                        }
                    }
                    else
                    {
                        //如果对方没有，直接加入
                        differenceConfig.Config.Add(item.Key, item.Value);
                    }
                }
                
                //如果没有不同，说明版本完全一样
                if (differenceConfig.Config.Count == 0)
                {
                    Debug.Log("Nothing Difference");
                    return null;
                }
                return differenceConfig;
            }
        }
        
        /// <summary>
        /// 包下载器，队列数据结构下载，提供添加下载任务接口
        /// </summary>
        private class BundleDownloaderTool
        {
            internal BundleDownloaderTool(Action onDownloadComplete, Action onAllFine, IHotUpdatePath pather)
            {
                OnDownloadComplete = onDownloadComplete;
                OnAllFine = onAllFine;
                this.pather = pather;
            }
            
            /// <summary>
            /// 添加下载任务
            /// </summary>
            /// <param name="bundle"></param>
            /// <param name="version"></param>
            internal void AddDownloadBundleCommand(string bundle, string version)
            {
                if (string.IsNullOrEmpty(bundle) || string.IsNullOrEmpty(version))
                    return;
                var downloadBundleCommand = new VersionedResource
                {
                    BundleName = bundle,
                    Version = version
                };
                downloadCommands.Push(downloadBundleCommand);
                if (!IsDownloading)
                    MainLoop.Instance.StartCoroutine(StartDownloading());
            }

            /// <summary>
            /// 判断资源包是否更行完毕
            /// </summary>
            /// <param name="checkConfig"></param>
            internal void CheckFileDone(ResourcesConfig checkConfig)
            {
                bool allFine = true;
                foreach (var item in checkConfig.Config)
                {
                    string checkpath = pather.GetLocalBundlePath(item.Key, item.Value);
                    if (!File.Exists(checkpath))
                    {
                        allFine = false;
                    }
                }

                if (OnAllFine != null && allFine)
                {
                    OnAllFine();
                }
            }


            #region Private
            
            //存储待下载的包
            private Stack<VersionedResource> downloadCommands = new Stack<VersionedResource>();
            private bool IsDownloading { get; set; }
            private Action OnDownloadComplete;
            private Action OnAllFine;
            private IHotUpdatePath pather;

            /// <summary>
            /// 下载资源包协程
            /// </summary>
            /// <returns></returns>
            private IEnumerator StartDownloading()
            {
                IsDownloading = true;
                while (downloadCommands.Count != 0)
                {
                    var cache = downloadCommands.Pop();
                    yield return DownloadAndSaveBundle(cache.BundleName, cache.Version);
                }

                OnDownloadComplete?.Invoke();
                IsDownloading = false;
            }

            /// <summary>
            /// 从服务器下载并保存新资源包
            /// </summary>
            /// <param name="bundleName"></param>
            /// <param name="bundleVersion"></param>
            /// <returns></returns>
            private IEnumerator DownloadAndSaveBundle(string bundleName, string bundleVersion)
            {
                //获取服务器请求url
                var requestUrl = pather.GetServeBundlePath(bundleName, bundleVersion);
                var webBytesLoadRequest = UnityWebRequest.Get(requestUrl);
                yield return webBytesLoadRequest.SendWebRequest();
                byte[] bs = webBytesLoadRequest.downloadHandler.data;
                //获取本地包存储路径
                var localPath = pather.GetLocalBundlePath(bundleName, bundleVersion);
                //将服务器获得的新包数据保存到本地
                FileUtil.CreateFile(localPath, bs);
            }
            

            #endregion
            
            
//    private IEnumerator DownloadAndSaveManifest(string manifestName, string manifestVersion)
//    {
//        UnityWebRequest webBytesLoadRequest = UnityWebRequest.Get(PathCreator.Instance.GetServeBundlePath(manifestName, manifestVersion));
//        yield return webBytesLoadRequest.SendWebRequest();
//        byte[] bs = webBytesLoadRequest.downloadHandler.data;
//        string FilePath = PathCreator.Instance.GetLocalBundlePath(manifestName, manifestVersion);
//        FileSaver.SaveAssetsBundle(bs, FilePath);
//    }
        }        

        #endregion

    }
}