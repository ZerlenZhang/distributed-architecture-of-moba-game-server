using System;
using System.Collections.Generic;
using System.IO;
using FileUtil = ReadyGamerOne.Utility.FileUtil;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace ReadyGamerOne.EditorExtension
{
//#pragma warning disable CS0414
    public class AutoGenerateTool
#if UNITY_EDITOR
        : IEditorTools
#endif
    {
        public static void RegisterUi(Type outType)
        {
            AbstractPanel.RegisterPanels(outType);
        }

#if UNITY_EDITOR
        private static string Title = "快速启动工具";
        private static List<string> autoClassName = new List<string>();
        private static Dictionary<string, string> otherResPathDic = new Dictionary<string, string>();
        private static Dictionary<string, string> otherResFileNameDic = new Dictionary<string, string>();

        private static Dictionary<string, string> allResPathDic = new Dictionary<string, string>();
        private static Dictionary<string, string> allResFileNameDic = new Dictionary<string, string>();


        private static bool createGameMgr = true;
        private static bool createPanelClasses = true;

        private static bool createGlobalVar = false;
        private static bool createMessage = false;
        private static bool createSceneNameClass = false;
        private static ResourceManagerType _resourceManagerType;


        static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,
            string scriptNs)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            createPanelClasses = EditorGUILayout.Toggle("是否自动生成Panel类型", createPanelClasses);
            if (createPanelClasses)
                EditorGUILayout.LabelField("自动生成Panel文件路径", Application.dataPath + "/" + rootNs + "/" + viewNs);
            EditorGUILayout.Space();


            createGameMgr = EditorGUILayout.Toggle("是否生成GameMgr", createGameMgr);
            if (createGameMgr)
            {
                _resourceManagerType = (ResourceManagerType) EditorGUILayout.EnumPopup("资源管理类型", _resourceManagerType);
                EditorGUILayout.LabelField("自动生成" + rootNs + "Mgr文件路径",
                    Application.dataPath + "/" + rootNs + "/" + scriptNs);
                
            }
            EditorGUILayout.Space();


                createMessage = EditorGUILayout.Toggle("是否生成空Message常量类", createMessage);
                if (createMessage)
                    EditorGUILayout.LabelField("生成Message.cs路径",
                        Application.dataPath + "/" + rootNs + "/" + constNs + "/Message.cs");
                EditorGUILayout.Space();
                createGlobalVar = EditorGUILayout.Toggle("是否重写GlobalVar类", createGlobalVar);
                if (createGlobalVar)
                    EditorGUILayout.LabelField("生成GlobalVar.cs路径",
                        Application.dataPath + "/" + rootNs + "/Global/GlobalVar.cs");
                EditorGUILayout.Space();
                createSceneNameClass = EditorGUILayout.Toggle("是否生成SceneName类", createSceneNameClass);
                if (createSceneNameClass)
                    EditorGUILayout.LabelField("生成SceneName.cs路径",
                        Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir + "/SceneName.cs");
                EditorGUILayout.Space();
            


            if (GUILayout.Button("开启自动生成", GUILayout.Height(2 * EditorGUIUtility.singleLineHeight)))
            {
                #region 文件夹的创建

                var resourceDir = Application.dataPath + "/Resources";
                var rootDir = Application.dataPath + "/" + rootNs;
                if (!Directory.Exists(resourceDir))
                {
                    Directory.CreateDirectory(resourceDir);
                    return;
                }

                FileUtil.CreateFolder(rootDir);
                FileUtil.CreateFolder(rootDir + "/" + constNs);
                FileUtil.CreateFolder(rootDir + "/" + constNs + "/" + autoDir);

                #endregion

                autoClassName.Clear();
                otherResPathDic.Clear();
                otherResFileNameDic.Clear();
                allResPathDic.Clear();
                allResFileNameDic.Clear();


                foreach (var fullName in Directory.GetFileSystemEntries(resourceDir))
                {
                    if (Directory.Exists(fullName))
                    {
                        //如果是文件夹
                        OprateDir(new DirectoryInfo(fullName));
                    }
                    else
                    {
                        //是文件
                        OprateFile(new FileInfo(fullName));
                    }
                }


                #region 特殊类的生成

                if (createPanelClasses && autoClassName.Contains("Panel"))
                    CreatePanelFile(Application.dataPath + "/Resources/ClassPanel", viewNs, constNs, rootNs, autoDir);

                #endregion


                #region 定向生成特殊小文件

                if (createGameMgr)
                {
                    CreateMgr(rootNs, constNs, scriptNs, autoDir);
                }

                if (createGlobalVar)
                    FileUtil.CreateClassFile("GlobalVar", rootNs + ".Global", rootDir + "/Global",
                        "ReadyGamerOne.Global.GlobalVar",
                        "这里写当前项目需要的全局变量，调用GlobalVar时，最好调用当前这个类");

                if (createMessage)
                    FileUtil.CreateClassFile("Message", rootNs + "." + constNs, rootDir + "/" + constNs,
                        helpTips: "自定义的消息写到这里");

                if (createSceneNameClass)
                    CreateSceneNameClass(rootNs, constNs, autoDir);

                #endregion


                AssetDatabase.Refresh();
                Debug.Log("生成结束");
            }

            #region 显示生成详情

            EditorGUILayout.Space();
            if (autoClassName.Count > 0)
            {
                var str = "生成的类型有：";
                foreach (var name in autoClassName)
                {
                    str += "\n" + name + "Name" + "\t\t" + name + "Path";
                }

                EditorGUILayout.HelpBox(str, MessageType.Info);
            }

            #endregion
        }
//        foreach (var name in names)
//        {
//            if(name.GetAfterLastChar('/')=="self")
//                continue;
//            otherClassBody += "\t\tpublic const string "+name.Trim().GetAfterLastChar('/')+" = " +
//                              "Path.Combine(Application.streamingAssetsPath, @\""+ name.Trim() +"\");\n";
//        }


        /// <summary>
        /// 创建场景名文件
        /// </summary>
        /// <param name="rootNs"></param>
        /// <param name="constNs"></param>
        /// <param name="autoDirName"></param>
        private static void CreateSceneNameClass(string rootNs, string constNs, string autoDirName)
        {
            var dic = new Dictionary<string, string>();
            foreach (EditorBuildSettingsScene S in EditorBuildSettings.scenes)
            {
                var path = S.path;
                var name = Path.GetFileNameWithoutExtension(path);
                dic.Add(name, name);
            }

            FileUtil.CreateConstClassByDictionary(
                "SceneName",
                Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDirName,
                rootNs + "." + constNs,
                dic);
        }


        /// <summary>
        /// 创建GameMgr
        /// </summary>
        /// <param name="rootNs"></param>
        /// <param name="constNs"></param>
        /// <param name="scriptNs"></param>
        /// <param name="autoDirName"></param>
        private static void CreateMgr(string rootNs, string constNs, string scriptNs, string autoDirName)
        {
            var safePartDir = Application.dataPath + "/" + rootNs + "/" + scriptNs;
            var autoPartDir = safePartDir + "/" + autoDirName;
            FileUtil.CreateFolder(safePartDir);
            FileUtil.CreateFolder(autoPartDir);
            var fileName = rootNs + "Mgr";

            var safePartPath = safePartDir + "/" + fileName + ".cs";
            var autoPartPath = autoPartDir + "/" + fileName + ".cs";

            #region AutoPart

            if (File.Exists(autoPartPath))
                File.Delete(autoPartPath);

            var stream = new StreamWriter(autoPartPath);

            stream.Write("using ReadyGamerOne.Script;\n");
            if(_resourceManagerType==ResourceManagerType.AssetBundle)
                stream.Write("using "+rootNs+"."+constNs+";\n");

            stream.Write("using ReadyGamerOne.MemorySystem;\n");
            

            stream.Write("namespace " + rootNs + "." + scriptNs + "\n" +
                         "{\n" +
                         "\tpublic partial class " + fileName + " : AbstractGameMgr<" + fileName + ">\n" +
                         "\t{\n");

            switch (_resourceManagerType)
            {
                case ResourceManagerType.Resource:
                    stream.Write("\t\tprotected override IResourceLoader ResourceLoader => ResourcesResourceLoader.Instance;\n" +
                                 "\t\tprotected override IAssetConstUtil AssetConstUtil => Utility.AssetConstUtil.Instance;\n");
                    break;
                case ResourceManagerType.AssetBundle:      
                    stream.Write("\t\tprotected override IResourceLoader ResourceLoader => AssetBundleResourceLoader.Instance;\n" +
                                 "\t\tprotected override IHotUpdatePath PathData => HotUpdatePathData.Instance;\n"+
                                 "\t\tprotected override IOriginAssetBundleUtil OriginBundleData => OriginBundleUtil.Instance;\n");
                    break;
            }



            stream.Write("\t\tpartial void OnSafeAwake();\n");

            stream.Write("\t\tprotected override void Awake()\n" +
                         "\t\t{\n" +
                         "\t\t\tbase.Awake();\n" +
                         "\t\t\tOnSafeAwake();\n");

            stream.Write("\t\t}\n" +
                         "\t}\n" +
                         "}\n");

            stream.Dispose();
            stream.Close();

            #endregion

            #region SafePart

            if (File.Exists(safePartPath))
                return;

            stream = new StreamWriter(safePartPath);

            var usePanel = autoClassName.Contains("Panel");
            var useAudio = autoClassName.Contains("Audio");
            if (useAudio || usePanel)
            {
                stream.Write("using ReadyGamerOne.EditorExtension;\n");
                stream.Write("using " + rootNs + "." + constNs + ";\n");
            }

            if (usePanel)
                stream.Write("using ReadyGamerOne.View;\n" +
                             "using ReadyGamerOne.Script;\n");

            stream.Write("namespace " + rootNs + "." + scriptNs + "\n" +
                         "{\n" +
                         "\tpublic partial class " + fileName + "\n" +
                         "\t{\n");
            if (usePanel)
            {
                stream.Write("\t\tpublic StringChooser startPanel = new StringChooser(typeof(PanelName));\n");
            }

            if (useAudio)
            {
                stream.Write("\t\tpublic StringChooser startBgm = new StringChooser(typeof(AudioName));\n");
            }

            stream.Write("\t\tpartial void OnSafeAwake()\n" +
                         "\t\t{\n");

            if (usePanel)
                stream.Write("\t\t\tPanelMgr.PushPanel(startPanel.StringValue);\n");
            if (useAudio)
                stream.Write("\t\t\tAudioMgr.Instance.PlayBgm(startBgm.StringValue);\n");

            stream.Write("\t\t\t//do any thing you want\n" +
                         "\t\t}\n" +
                         "\t}\n" +
                         "}\n");


            stream.Dispose();
            stream.Close();

            #endregion
        }

        /// <summary>
        /// 创建Panel类
        /// </summary>
        /// <param name="panelPrefabDir"></param>
        /// <param name="viewNs"></param>
        /// <param name="constNs"></param>
        /// <param name="rootNs"></param>
        /// <param name="autoDirName"></param>
        /// <returns></returns>
        private static bool CreatePanelFile(string panelPrefabDir, string viewNs, string constNs, string rootNs,
            string autoDirName)
        {
            if (!Directory.Exists(panelPrefabDir))
            {
                Debug.LogWarning("panelPrefabDir is not exist ! ");
                return false;
            }

            foreach (var fullPath in Directory.GetFiles(panelPrefabDir))
            {
                if (fullPath.EndsWith(".meta"))
                    continue;
                if (!fullPath.EndsWith(".prefab"))
                    continue;

                var prefabName = Path.GetFileNameWithoutExtension(fullPath);
                var fileName = prefabName;
                var className = prefabName;
                var safePartDir = Application.dataPath + "/" + rootNs + "/" + viewNs;
                var autoPartDir = safePartDir + "/" + autoDirName;
//                Debug.Log(safePartDir);
                FileUtil.CreateFolder(safePartDir);
                FileUtil.CreateFolder(autoPartDir);
                var safePartPath = safePartDir + "/" + fileName + ".cs";
                var autoPartPath = autoPartDir + "/" + fileName + ".cs";

                #region AutoPart

                if (File.Exists(autoPartPath))
                    File.Delete(autoPartPath);

                var stream = File.CreateText(autoPartPath);

                stream.Write("using ReadyGamerOne.View;\n" +
                             "using " + rootNs + "." + constNs + ";\n" +
                             "namespace " + rootNs + "." + viewNs + "\n" +
                             "{\n" +
                             "\tpublic partial class " + className + " : AbstractPanel\n" +
                             "\t{\n" +
                             "\t\tpartial void OnLoad();\n" +
                             "\n" +
                             "\t\tprotected override void Load()\n" +
                             "\t\t{\n" +
                             "\t\t\tCreate(PanelName." + className + ");\n" +
                             "\t\t\tOnLoad();\n" +
                             "\t\t}\n" +
                             "\t}\n" +
                             "}\n");


                stream.Flush();
                stream.Dispose();
                stream.Close();

                #endregion

                #region SafePart

                if (File.Exists(safePartPath))
                    continue;

                stream = new StreamWriter(safePartPath);

                stream.Write("namespace " + rootNs + "." + viewNs + "\n" +
                             "{\n" +
                             "\tpublic partial class " + fileName + "\n" +
                             "\t{\n" +
                             "\t\tpartial void OnLoad()\n" +
                             "\t\t{\n" +
                             "\t\t\t//do any thing you want\n" +
                             "\t\t}\n" +
                             "\t}\n" +
                             "}\n");

                stream.Dispose();
                stream.Close();

                #endregion
            }

            return true;
        }

        /// <summary>
        /// 遍历Resources目录的时候操作文件的函数
        /// </summary>
        /// <param name="fileInfo"></param>
        private static void OprateFile(FileInfo fileInfo)
        {
//            Debug.Log(fileInfo.FullName);
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
        private static bool OprateDir(DirectoryInfo dirInfo)
        {
            var dirName = dirInfo.FullName.GetAfterLastChar('\\');

            if (dirName == "Resources")
                return true;

            if (dirName.StartsWith("Global"))
                return false;
            
            if (dirName.StartsWith("Class"))
            {
                autoClassName.Add(dirName.GetAfterSubstring("Class"));

                FileUtil.SearchDirectory(dirInfo.FullName,
                    fileInfo =>
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
                        }
                    },true);
            }

            return true;
        }
#endif
    }
}