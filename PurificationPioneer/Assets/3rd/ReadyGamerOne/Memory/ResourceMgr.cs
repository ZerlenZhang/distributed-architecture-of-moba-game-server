using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReadyGamerOne.Const;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using FileUtil = ReadyGamerOne.Utility.FileUtil;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System.IO;
using ReadyGamerOne.Global;
using UnityEditor;

#endif


namespace ReadyGamerOne.MemorySystem
{
    public enum ResourceManagerType
         {
             Resource,
             AssetBundle
         }

    /// <summary>
    /// 这个类提供关于内存的优化和管理
    /// 1、所有资源只会从Resources目录加载一次，再取的时候会从这个类的字典中取，尤其是一些预制体，经常频繁加载，使用这个类封装的Instantiate方法可以很好地解决这个问题
    /// 2、提供一些释放资源的接口
    /// 3、以后会提供关于AssetBubble的方法和接口
    /// 4、提供从Resources目录运行时加载一整个目录资源的接口，比如，加载某个文件夹下所有图片，音频
    /// </summary>
    public class ResourceMgr
#if UNITY_EDITOR
        : IEditorTools
#endif
    {
        #region Fields

        private static IResourceLoader _resourceLoader;
        private static IAssetConstUtil _assetConstUtil;

        public static void Init(IResourceLoader resourceLoader, IHotUpdatePath pather,
            IOriginAssetBundleUtil originConstData, IAssetConstUtil assetConstUtil)
        {
            Assert.IsNotNull(resourceLoader);
            Assert.IsNotNull(assetConstUtil);
            _assetConstUtil = assetConstUtil;
            _resourceLoader = resourceLoader;
//            Debug.Log("初始化");
            _resourceLoader.Init(pather, originConstData, assetConstUtil);
        }

        #endregion


        /// <summary>
        /// 根据objKey获取路径
        /// </summary>
        /// <param name="objKey"></param>
        /// <returns></returns>
        public string NameToPath(string objKey)
        {
            string path = null;
            return !_assetConstUtil.NameToPath.TryGetValue(objKey, out path) ? null : path;
        }

        /// <summary>
        /// 显示调试信息
        /// </summary>
        public static void ShowDebugInfo()
        {
            var abl = _resourceLoader as AssetBundleResourceLoader;
            abl?.ShowDebugInfo();
        }


        public static T GetAsset<T>(string objKey, string bundleName = null)
            where T : UnityEngine.Object
        {
            return _resourceLoader.GetAsset<T>(objKey, bundleName);
        }

        public static IEnumerator GetAssetAsync<T>(string objName, string bundleKey = null, Action<T> onGetObj = null)
            where T : UnityEngine.Object
        {
            yield return _resourceLoader.GetAssetAsync<T>(objName, bundleKey, onGetObj);
        }

        
   
        /// <summary>
        /// 异步从本地读取AssetBundle
        /// 条件：1、AB包没有做过任何加密
        ///       2、AB包存储在本地
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="crc"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IEnumerator LoadAssetBundleFromLocalAsync(string filePath, uint crc = 0u, ulong offset = 0ul)
        {
            yield return AssetBundle.LoadFromFileAsync(filePath, crc, offset);
        }

        /// <summary>
        /// 同步从本地读取AssetBundle
        /// 条件：1、AB包没有做过任何加密
        ///       2、AB包存储在本地
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="crc"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundleFromLocal(string filePath, uint crc = 0u, ulong offset = 0ul)
        {
            return AssetBundle.LoadFromFile(filePath, crc, offset);
        }
     
        
        
        

        /// <summary>
        /// 释放游离资源
        /// </summary>
        public static void ReleaseUnusedAssets()
        {
            _resourceLoader.ClearCache();
            Resources.UnloadUnusedAssets();
        }


        /// <summary>
        /// 根据路径实例化对象
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instantiate<T>(string objName) where T : Object
        {
            return Object.Instantiate(_resourceLoader.GetAsset<T>(objName, OriginBundleKey.Self));
        }

        #region 实例化GameObject

        public static GameObject InstantiateGameObject(string objName, Transform parent = null)
        {
            var source = _resourceLoader.GetAsset<GameObject>(objName);
            Assert.IsTrue(source);
            return Object.Instantiate(source, parent);
        }

        public static GameObject InstantiateGameObject(string objName, Vector3 worldPos, Quaternion quaternion,
            Transform parent = null)
        {
            Assert.IsNotNull(_resourceLoader);
            var source = _resourceLoader.GetAsset<GameObject>(objName);
            Assert.IsTrue(source);
            return Object.Instantiate(source, worldPos, quaternion, parent);
        }

        public static GameObject InstantiateGameObject(string objName, Vector3 worldPos, Transform parent = null)
        {
            var source = _resourceLoader.GetAsset<GameObject>(objName);
            Assert.IsTrue(source);
            return Object.Instantiate(source, worldPos, Quaternion.identity, parent);
        }
        

        #endregion

        /// <summary>
        /// 从Resources目录中动态加载指定目录下所有内容
        /// </summary>
        /// <param name="nameClassType">目录下所有资源的名字要作为一个静态类的public static 成员，这里传递 这个静态类的Type</param>
        /// <param name="dirPath">从Resources开始的根目录，比如“Audio/"</param>
        /// <param name="onLoadAsset">加载资源时调用的委托，不能为空</param>
        /// <typeparam name="TAssetType">加载资源的类型</typeparam>
        /// <exception cref="Exception">委托为空会抛异常</exception>
        public static void LoadAssetFromResourceDir<TAssetType>(Type nameClassType, string dirPath = "",
            Action<string, TAssetType> onLoadAsset = null)
            where TAssetType : Object
        {
            var infoList = nameClassType.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var fieldInfo in infoList)
            {
                string assetName = fieldInfo.GetValue(null) as string;

                var res = _resourceLoader.GetAsset<TAssetType>(dirPath + assetName);

                if (onLoadAsset == null)
                    throw new Exception("onLoadAsset为 null, 那你加载资源干啥？？ ");

                onLoadAsset.Invoke(assetName, res);
            }
        }

        #region IEditorTools

#if UNITY_EDITOR
#pragma warning disable 414
        static string Title = "资源管理";
#pragma warning restore 414

        private static ResourceManagerType _resourceManagerType = ResourceManagerType.Resource;
       
        #region AssetBundle_Auto_Generate

        private static string assetToBundleDir = "未设置";
        private static string outputDir = "未设置";
        private static BuildAssetBundleOptions assetBundleOptions = 0;
        private static BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        private static bool clearOutputDir = false;

        private static bool createPathDataClass = false;

        private static bool useForRuntime = true;

        private static string streamingAbName = "self";
        private static bool useWeb = false;
        private static List<string> assetBundleNames = new List<string>();        

        #endregion

        #region Resources_Auto_Generate

        private static List<string> autoClassName = new List<string>();
        private static Dictionary<string, string> otherResPathDic = new Dictionary<string, string>();
        private static Dictionary<string, string> otherResFileNameDic = new Dictionary<string, string>();

        private static Dictionary<string, string> allResPathDic = new Dictionary<string, string>();
        private static Dictionary<string, string> allResFileNameDic = new Dictionary<string, string>();

        private static bool createPathFile = false;
        #endregion

        static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,
            string scriptDir)
        {
            
            EditorGUILayout.Space();
            _resourceManagerType = (ResourceManagerType) EditorGUILayout.EnumPopup("资源加载类型", _resourceManagerType);

            switch (_resourceManagerType)
            {
                #region ResourceManagerType.Resource

                case ResourceManagerType.Resource:

                    EditorGUILayout.LabelField("自动常量文件生成目录",
                        Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir);
                    EditorGUILayout.LabelField("常量工具类生成目录",
                        Application.dataPath + "/" + rootNs + "/Utility/" + autoDir + "/ConstUtil.cs");
                    EditorGUILayout.Space();

                    createPathFile = EditorGUILayout.Toggle("是否生成资源路径文件", createPathFile);
                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("生成常量"))
                    {
                        GenerateResourcesConst(rootNs,constNs,autoDir);
                    }

                    break;

                #endregion


                #region ResourceManagerType.AssetBundle

                case ResourceManagerType.AssetBundle:

                    var newestVersion = PlayerPrefs.GetString(VersionDefine.PrefKey_LocalVersion, "0.0");

                    if (GUILayout.Button("显示本地版本"))
                    {
                        var versionData = PlayerPrefs.GetString(newestVersion);
                        Debug.Log("本地版本号：" + newestVersion + "\n版本信息：" + versionData);
                    }

                    EditorGUI.BeginChangeCheck();
                    newestVersion = EditorGUILayout.DelayedTextField("本地版本", newestVersion);
                    if (EditorGUI.EndChangeCheck())
                    {
                        PlayerPrefs.SetString(VersionDefine.PrefKey_LocalVersion, newestVersion);
                    }
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("打包目录", assetToBundleDir);
                    if (GUILayout.Button("设置要打包的目录"))
                    {
                        assetToBundleDir = EditorUtility.OpenFolderPanel("选择需要打包的目录", Application.dataPath, "");
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("输出目录", outputDir);
                    if (GUILayout.Button("设置输出目录"))
                    {
                        outputDir = EditorUtility.OpenFolderPanel("选择输出目录", Application.dataPath, "");
                    }

                    EditorGUILayout.Space();
                    assetBundleOptions =
                        (BuildAssetBundleOptions) EditorGUILayout.EnumFlagsField("打包选项", assetBundleOptions);

                    buildTarget = (BuildTarget) EditorGUILayout.EnumPopup("目标平台", buildTarget);
                    clearOutputDir = EditorGUILayout.Toggle("清空生成目录", clearOutputDir);
                    EditorGUILayout.Space();

                    useForRuntime = EditorGUILayout.Foldout(useForRuntime, "使用用于生成运行时直接使用的AB包");
                    if (useForRuntime)
                    {
                        EditorGUILayout.LabelField("生成HotUpdatePath.cs路径",
                            Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir + "/HotUpdatePath.cs");
                        useWeb = EditorGUILayout.Toggle("是否使用网络", useWeb);
                        EditorGUILayout.LabelField("游戏自身AB包名字", streamingAbName);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("重新生成常量类【会覆盖】"))
                        {
                            if (!outputDir.Contains(Application.streamingAssetsPath))
                            {
                                Debug.LogError("运行时使用的AB包必须在StreamingAssets目录下");
                                return;
                            }

                            if (assetBundleNames.Count == 0)
                            {
                                Debug.LogError("AB包数组未空");
                                return;
                            }

                            CreatePathDataClass(rootNs, constNs, autoDir, assetBundleNames);
                            AssetDatabase.Refresh();
                            Debug.Log("生成完成");
                        }
                    }


                    if (GUILayout.Button("开始打包", GUILayout.Height(3 * EditorGUIUtility.singleLineHeight)))
                    {
                        if (createPathDataClass)
                        {
                            if (!outputDir.Contains(Application.streamingAssetsPath))
                            {
                                Debug.LogError("运行时使用的AB包必须在StreamingAssets目录下");
                                return;
                            }
                        }

                        if (assetBundleNames.Count != 0)
                            assetBundleNames.Clear();
                        if (!Directory.Exists(assetToBundleDir))
                        {
                            Debug.LogError("打包目录设置异常");
                            return;
                        }

                        if (!Directory.Exists(outputDir))
                        {
                            Debug.LogError("输出目录设置异常");
                            return;
                        }

                        if (clearOutputDir)
                        {
                            if (Directory.Exists(outputDir))
                            {
                                //获取指定路径下所有文件夹
                                string[] folderPaths = Directory.GetDirectories(outputDir);

                                foreach (string folderPath in folderPaths)
                                    Directory.Delete(folderPath, true);
                                //获取指定路径下所有文件
                                string[] filePaths = Directory.GetFiles(outputDir);

                                foreach (string filePath in filePaths)
                                    File.Delete(filePath);
                            }
                        }

                        var builds = new List<AssetBundleBuild>();
                        foreach (var dirPath in System.IO.Directory.GetDirectories(assetToBundleDir))
                        {
                            var dirName = new DirectoryInfo(dirPath).Name;
                            var paths = new List<string>();
                            var assetNames = new List<string>();
                            FileUtil.SearchDirectory(dirPath,
                                fileInfo =>
                                {
                                    if (fileInfo.Name.EndsWith(".meta"))
                                        return;
                                    var pureName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                                    assetNames.Add(pureName);
                                    var fileLoadPath = fileInfo.FullName.Replace("\\", "/")
                                        .Replace(Application.dataPath, "Assets");
                                    var ai = AssetImporter.GetAtPath(fileLoadPath);
                                    ai.assetBundleName = dirName;
                                    paths.Add(fileLoadPath);
                                }, true);

                            assetBundleNames.Add(dirName);
                            FileUtil.CreateConstClassByDictionary(
                                dirName + "Name",
                                Application.dataPath + "/" + rootNs + "/" + constNs,
                                rootNs + "." + constNs,
                                assetNames.ToDictionary(name => name));
                            builds.Add(new AssetBundleBuild
                            {
                                assetNames = paths.ToArray(),
                                assetBundleName = dirName
                            });
                        }

                        if (createPathDataClass)
                        {
                            CreatePathDataClass(rootNs, constNs, autoDir, assetBundleNames);
                        }


                        BuildPipeline.BuildAssetBundles(outputDir, builds.ToArray(), assetBundleOptions, buildTarget);
                        AssetDatabase.Refresh();
                        Debug.Log("打包完成");
                    }

                    break;

                #endregion
            }
        }

        public static void GenerateResourcesConst(string rootNs,string constNs,string autoDir)
        {
             var resourceDir = Application.dataPath + "/Resources";
             var rootDir = Application.dataPath + "/" + rootNs;
             
             autoClassName.Clear();
             otherResPathDic.Clear();
             otherResFileNameDic.Clear();
             allResPathDic.Clear();
             allResFileNameDic.Clear();

             if (!Directory.Exists(resourceDir))
             {
                 Directory.CreateDirectory(resourceDir);
             }


             foreach (var fullName in Directory.GetFileSystemEntries(resourceDir))
             {
//                 Debug.Log(fullName);
                 if (Directory.Exists(fullName))
                 {
                     //如果是文件夹
                     OprateDir(new DirectoryInfo(fullName), rootNs, constNs, autoDir);
                 }
                 else
                 {
                     //是文件
                     OprateFile(new FileInfo(fullName));
                 }
             }

             //生成其他常量文件
             if (otherResPathDic.Count > 0)
             {
//                 Debug.Log("创建文件OtherResPath");
                 FileUtil.CreateConstClassByDictionary("OtherResPath",
                     rootDir + "/" + constNs + "/" + autoDir,
                     rootNs + "." + constNs, otherResPathDic);
                 FileUtil.CreateConstClassByDictionary("OtherResName",
                     rootDir + "/" + constNs + "/" + autoDir,
                     rootNs + "." + constNs, otherResFileNameDic);
                 autoClassName.Add("OtherRes");
             }

             //生成常量工具类
             if (allResPathDic.Count > 0)
             {
                 var content =
                     "\t\tprivate System.Collections.Generic.Dictionary<string,string> nameToPath \n" +
                     "\t\t\t= new System.Collections.Generic.Dictionary<string,string>{\n";

                 foreach (var kv in allResFileNameDic)
                 {
                     content += "\t\t\t\t\t{ @\"" + kv.Value + "\" , @\"" + allResPathDic[kv.Key] +
                                "\" },\n";
                 }

                 content += "\t\t\t\t};\n";

                 content +=
                     "\t\tpublic override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;\n";

                 FileUtil.CreateClassFile("AssetConstUtil",
                     rootNs + ".Utility",
                     rootDir + "/Utility/" + autoDir,
                     parentClass:"ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>",
                     helpTips: "这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug",
                     fileContent: content,
                     autoOverwrite: true);
             }

             
             AssetDatabase.Refresh();
             Debug.Log("生成结束");
        }


        /// <summary>
        /// 创建用于使用AB包管理资源的路径定义类
        /// </summary>
        /// <param name="rootNs"></param>
        /// <param name="constNs"></param>
        /// <param name="autoDir"></param>
        private static void CreatePathDataClass(string rootNs, string constNs, string autoDir, List<string> names)
        {
            var outputDir = ResourceMgr.outputDir.Replace(Application.streamingAssetsPath, "");
            var mainAssetBundleName = new DirectoryInfo(ResourceMgr.outputDir).Name;
            var content = "";
            var otherClassBody = "\n";

            #region OriginBundleKey

            otherClassBody += "\tpublic class OriginBundleKey : ReadyGamerOne.MemorySystem.OriginBundleKey\n" +
                              "\t{\n";
            foreach (var name in names)
            {
                var pureName = name.GetAfterLastChar('/');
                if (pureName == "Audio" || pureName == "File")
                    continue;
                otherClassBody += "\t\tpublic static readonly string " + name.Trim().GetAfterLastChar('/') + " = " +
                                  "@\"" + name.Trim() + "\";\n";
            }

            otherClassBody += "\t}\n";

            #endregion

            #region OriginBundleUtil

            otherClassBody += "\n\tpublic class OriginBundleUtil : OriginBundleUtil<OriginBundleUtil>\n" +
                              "\t{\n";

            #region KeyToName

            otherClassBody += "\t\tprivate Dictionary<string,string> _keyToName = new Dictionary<string,string>\n" +
                              "\t\t{\n" +
                              "\t\t\t{\"Self\" , @\"self\"},\n";
            foreach (var name in names)
            {
                if (name.GetAfterLastChar('/') == "self")
                    continue;
                otherClassBody += "\t\t\t{\"" + name.Trim().GetAfterLastChar('/') + "\" , " +
                                  "@\"" + name.Trim().ToLower() + "\"},\n";
            }

            otherClassBody += "\t\t};\n";
            otherClassBody += "\t\tpublic override Dictionary<string, string> KeyToName => _keyToName;\n";

            #endregion

            #region KeyToPath

            otherClassBody += "\t\tprivate Dictionary<string,string> _keyToPath = new Dictionary<string,string>\n" +
                              "\t\t{\n" +
                              "\t\t\t{\"Self\" , Path.Combine(Application.streamingAssetsPath + @\"" + outputDir +
                              "\", @\"self\")},\n";
            foreach (var name in names)
            {
                if (name.GetAfterLastChar('/') == "self")
                    continue;
                otherClassBody += "\t\t\t{\"" + name.Trim().GetAfterLastChar('/') + "\" , " +
                                  "Path.Combine(Application.streamingAssetsPath + @\"" + outputDir + "\", @\"" +
                                  name.Trim().ToLower() + "\")},\n";
            }

            otherClassBody += "\t\t};\n";
            otherClassBody += "\t\tpublic override Dictionary<string, string> KeyToPath => _keyToPath;\n";

            #endregion

            #region NameToPath

            otherClassBody += "\t\tprivate Dictionary<string,string> _nameToPath = new Dictionary<string,string>\n" +
                              "\t\t{\n" +
                              "\t\t\t{@\"self\" , Path.Combine(Application.streamingAssetsPath + @\"" + outputDir +
                              "\", @\"self\")},\n";
            foreach (var name in names)
            {
                if (name.GetAfterLastChar('/') == "self")
                    continue;
                otherClassBody += "\t\t\t{@\"" + name.Trim().ToLower() + "\" , " +
                                  "Path.Combine(Application.streamingAssetsPath + @\"" + outputDir + "\", @\"" +
                                  name.Trim().ToLower() + "\")},\n";
            }

            otherClassBody += "\t\t};\n";
            otherClassBody += "\t\tpublic override Dictionary<string, string> NameToPath => _nameToPath;\n";

            #endregion

            otherClassBody += "\t}\n";

            #endregion

            if (useWeb)
            {
                content =
                    "\t\tpublic string OriginMainManifest => @Path.Combine(Application.streamingAssetsPath + @\"" +
                    outputDir + "\", \"" +
                    mainAssetBundleName + "\");\n" +
                    "\t\tpublic string LocalMainPath => @Path.Combine(Application.persistentDataPath, \"AssetBundles\");\n" +
                    "\t\tpublic string WebServeMainPath => @\"file:/C:\\Users\\ReadyGamerOne\\Downloads\\webserver\";\n" +
                    "\t\tpublic string WebServeMainManifest => WebServeMainPath + \"\\\\ManifestFile\";\n" +
                    "\t\tpublic string WebServeVersionPath => WebServeMainPath + \"\\\\ServeVersion.html\";\n" +
                    "\t\tpublic string WebServeBundlePath => WebServeMainPath + \"\\\\AssetBundles\";\n" +
                    "\t\tpublic string WebServeConfigPath => WebServeMainPath + \"\\\\ServeConfig\";\n" +
                    "\t\tpublic Func<string, string> GetServeConfigPath => version =>$\"{WebServeConfigPath}/{version}.html\";\n" +
                    "\t\tpublic Func<string, string, string> GetServeBundlePath => (bundleName,bundleVersion)=>$\"{WebServeBundlePath}/{bundleVersion}/{bundleName}\";\n" +
                    "\t\tpublic Func<string, string, string> GetLocalBundlePath => (bundleName,bundleVersion)=>$\"{LocalMainPath}/{bundleVersion}/{bundleName}\";\n";
            }
            else
            {
                content =
                    "\t\tpublic string OriginMainManifest => @Path.Combine(Application.streamingAssetsPath + @\"" +
                    outputDir + "\", \"" +
                    mainAssetBundleName + "\");\n" +
                    "\t\tpublic string LocalMainPath => null;\n" +
                    "\t\tpublic string WebServeMainPath => null;\n" +
                    "\t\tpublic string WebServeMainManifest => null;\n" +
                    "\t\tpublic string WebServeVersionPath => null;\n" +
                    "\t\tpublic string WebServeBundlePath => null;\n" +
                    "\t\tpublic string WebServeConfigPath => null;\n" +
                    "\t\tpublic Func<string, string> GetServeConfigPath => null;\n" +
                    "\t\tpublic Func<string, string, string> GetServeBundlePath => null;\n" +
                    "\t\tpublic Func<string, string, string> GetLocalBundlePath => null;\n";
            }

            FileUtil.CreateClassFile(
                "HotUpdatePathData",
                rootNs + "." + constNs,
                Application.dataPath + "/" + rootNs + "/" + constNs,
                "Singleton<HotUpdatePathData>, IHotUpdatePath",
                "使用AB包管理资源时必须的定义路径的常量类",
                content,
                true,
                false,
                "using ReadyGamerOne.MemorySystem;\n" +
                "using UnityEngine;\n" +
                "using System.IO;\n" +
                "using System;\n" +
                "using System.Collections.Generic;\n" +
                "using ReadyGamerOne.Common;\n",
                otherClassBody: otherClassBody);
        }

        /// <summary>
        /// 遍历Resources目录的时候操作文件的函数
        /// </summary>
        /// <param name="fileInfo"></param>
        private static void OprateFile(FileInfo fileInfo)
        {
            if (fileInfo.FullName.EndsWith(".meta"))
            {
//                Debug.Log("跳过。meta");
                return;
            }

            var loadPath = fileInfo.FullName.GetAfterSubstring("Resources\\")
                .GetBeforeSubstring(Path.GetExtension(fileInfo.FullName));
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            var varName = FileUtil.FileNameToVarName(fileName);

            if (allResPathDic.ContainsKey(varName))
            {
                Debug.LogWarning("相同Key: " + varName);
                Debug.LogWarning("已存：" + allResPathDic[varName]);
                Debug.LogWarning("现在：" + loadPath);
            }
            else
            {
//                Debug.Log("添加了");
                allResPathDic.Add(varName, loadPath);
                allResFileNameDic.Add(varName, fileName);

                otherResPathDic.Add(varName, loadPath);
                otherResFileNameDic.Add(varName, fileName);
            }
        }

        /// <summary>
        /// 遍历Resources目录时，操作目录的函数
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="rootNs"></param>
        /// <param name="constNs"></param>
        /// <param name="autoDir"></param>
        private static bool OprateDir(DirectoryInfo dirInfo, string rootNs, string constNs, string autoDir)
        {
//            Debug.Log(dirInfo.FullName);
            var dirName = dirInfo.FullName.GetAfterLastChar('\\');

            if (dirName == "Resources")
                return true;

//            Debug.Log("operateDir: " + dirName);
            if (dirName.StartsWith("Global"))
                return false;
            if (dirName.StartsWith("Class"))
            {
                autoClassName.Add(dirName.GetAfterSubstring("Class"));

                if (createPathFile)
                {
                         FileUtil.ReCreateFileNameConstClassFromDir(
                             dirName.GetAfterSubstring("Class") + "Path",
                             Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir,
                             dirInfo.FullName,
                             rootNs + "." + constNs,
                             (fileInfo, stream) =>
                             {
                                 if (!fileInfo.FullName.EndsWith(".meta"))
                                 {
                                     var fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                                     var varName = FileUtil.FileNameToVarName(fileName);
                                     var loadPath = fileInfo.FullName.GetAfterSubstring("Resources\\")
                                         .GetBeforeSubstring(Path.GetExtension(fileInfo.FullName));
                                     stream.Write("\t\tpublic static readonly string " + varName + " = @\"" + loadPath + "\";\n");
                                 }
                             }, true);           
                }


                var className = dirName.GetAfterSubstring("Class") + "Name";
                FileUtil.ReCreateFileNameConstClassFromDir(
                    className,
                    Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir,
                    dirInfo.FullName,
                    rootNs + "." + constNs,
                    (fileInfo, stream) =>
                    {
                        if (!fileInfo.FullName.EndsWith(".meta"))
                        {
                            var fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                            var varName = FileUtil.FileNameToVarName(fileName);
                            var loadPath = fileInfo.FullName.GetAfterSubstring("Resources\\")
                                .GetBeforeSubstring(Path.GetExtension(fileInfo.FullName));

                            if (allResPathDic.ContainsKey(varName))
                            {
                                Debug.LogWarning("出现同名资源文件：" + fileInfo);
                            }
                            else
                            {
                                allResPathDic.Add(varName, loadPath);
                                allResFileNameDic.Add(varName, fileName);
                            }

                            stream.Write("\t\tpublic static readonly string " + varName + " = @\"" + fileName + "\";\n");
                        }
                    }, true);
            }
            else
                FileUtil.SearchDirectory(dirInfo.FullName, OprateFile,true);

            return true;
        }

#endif

        #endregion
    }
}