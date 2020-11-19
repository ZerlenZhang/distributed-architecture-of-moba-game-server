using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReadyGamerOne.Global;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using DataConfigInfo=ReadyGamerOne.Data.CsvMgr.DataConfigInfo;
using FieldInfo=ReadyGamerOne.Data.CsvMgr.FieldInfo;

namespace ReadyGamerOne.Data
{
//#pragma warning disable CS0414
    
#if UNITY_EDITOR
    
    public class CsvDecodingTools:IEditorTools
    {
        private static string csvDirPath = "";


        private static GUIStyle titleStyle = new GUIStyle
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        };

        #pragma warning disable 414
        private static string Title = "CSV解析";
        #pragma warning restore 414
        private static string DataConfigFileName = "DataConfig";
        private static bool overriteOldFile = false;
        private static Dictionary<string, DataConfigInfo> _dataConfigInfos = new Dictionary<string, DataConfigInfo>();
        private static void OnToolsGUI(string rootNs,string viewNs,string constNs,string dataNs,string autoDir,string scriptDir)
        {
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("使用本工具需要满足以下条件：\n" +
                                    "1、创建一个存放数据结构定义文件和数据文件的目录\n" +
                                    "2、创建数据结构问价 "+DataConfigFileName+" 定义数据类名，Fields，和父类\n" +
                                    "3、创建 \"类名.csv\" 文件定义类数据\n" +
                                    "4、将目录制定到文件目录，开始生成",MessageType.Info);
            

            var generateDir = Application.dataPath + "/" + rootNs + "/" + dataNs + "/" + autoDir;
            
            EditorGUILayout.LabelField("csv数据类生成目录", rootNs + "/" + dataNs + "/" + autoDir);
            
            EditorGUILayout.Space();
            GUILayout.Label("请选择Csv文件所在目录",titleStyle);

            if(Directory.Exists(csvDirPath) )
                GUILayout.Label(csvDirPath);
            else
            {
                EditorGUILayout.HelpBox("请选择CSV目录路径", MessageType.Warning);
            }
            if (GUILayout.Button("设置CSV数据文件所在目录"))
                csvDirPath = EditorUtility.OpenFolderPanel("选择Csv文件所在目录", Directory.GetParent(Application.dataPath).FullName,"");
        
            EditorGUILayout.Space();
            overriteOldFile = EditorGUILayout.Toggle("是否覆盖旧文件", overriteOldFile);
            EditorGUILayout.Space();


            if (GUILayout.Button("根据DataConfig文件生成空csv文件"))
            {               
                if(!UnityEngine.Windows.Directory.Exists(csvDirPath))
                {
                    Debug.LogError("生成失败——请正确设置所有路径");
                    return;
                }

                if (!CreateMoreCsvFile())
                {
                    return;
                }
                AssetDatabase.Refresh();
                Debug.Log("生成完毕");
//                WindowsUtil.OpenFolderInExplorer(csvDirPath);
            }
            
            
            if (GUILayout.Button("生成C#协议文件",GUILayout.Height(3*EditorGUIUtility.singleLineHeight)))
            {
                if (!ReadDataConfigFile(true))
                {
                    Debug.Log("读取" + DataConfigFileName + "文件失败");
                    return;
                }
                var consDir = Application.dataPath + "/" + rootNs + "/" + constNs + "/" + autoDir;
                if (!Directory.Exists(consDir))
                    Directory.CreateDirectory(consDir);
                if (!Directory.Exists(generateDir))
                    Directory.CreateDirectory(generateDir);
                
                if (Directory.Exists(csvDirPath))
                {
                    foreach (var fileFullPath in Directory.GetFiles(csvDirPath))
                    {
                        if(fileFullPath.EndsWith(".meta"))
                            continue;
                        if(Path.GetFileNameWithoutExtension(fileFullPath)==DataConfigFileName)
                            continue;
                        if (fileFullPath.EndsWith(".csv") || fileFullPath.EndsWith(".CSV"))
                        {
                            CreatConfigFile(fileFullPath, generateDir,rootNs+"."+dataNs);                       
                        }
                    }
                    
         
                    Utility.FileUtil.ReCreateFileNameConstClassFromDir("FileName", consDir,Application.dataPath + "/Resources/ClassFile",rootNs+"."+constNs);               
                    AssetDatabase.Refresh();        //这里是一个点
                    Debug.Log("生成完成");
                }
                else
                {
                    Debug.LogError("生成失败——请正确设置所有路径");
                }
            }
        }

        /// <summary>
        /// 读取数据结构表信息
        /// </summary>
        /// <param name="copyToResource"></param>
        /// <returns></returns>
        private static bool ReadDataConfigFile(bool copyToResource=false)
        {
            var filePath = "";
            if (copyToResource)
            {
                var targetPath = Application.dataPath + "/Resources/ClassFile";

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);
                if (File.Exists(csvDirPath + "/" + DataConfigFileName + ".CSV"))
                {
                    filePath = csvDirPath + "/" + DataConfigFileName + ".CSV";
                }else if (File.Exists(csvDirPath + "/" + DataConfigFileName + ".csv"))
                {
                    filePath = csvDirPath + "/" + DataConfigFileName + ".csv";
                }
                else
                {
                    return false;
                }

                var fileName = Path.GetFileNameWithoutExtension(filePath);

                targetPath = targetPath + "/" + fileName + ".csv";
                if (new FileInfo(targetPath).FullName != new FileInfo(filePath).FullName)
                {
                    File.Copy(filePath, targetPath, true);
                }                
            }
            
            _dataConfigInfos.Clear();
            
            var csr = new CsvStreamReader(filePath);

            for (var i = 1; i < csr.RowCount + 1; i += 2)
            {
                var config = new DataConfigInfo();
                for (var j = 1; j < csr.ColCount + 1; j++)
                {
                    var name = csr[i+0, j];
                    var type = csr[i+1, j];

                    switch (type)
                    {
                        case "className":
                            config.className = name;
                            break;
                        case "parentClassName":
                            config.parentName = name;
                            break;
                        case "fileKeys":
                            foreach (var fileKey in name.Split('_'))
                            {
                                config.fileKeys.Add(fileKey.Trim());
                            }
                            break;
                        default:
                            if (string.IsNullOrEmpty(name))
                                break;
                            config.fieldInfos.Add(
                                name,
                                new FieldInfo
                            {
                                fieldName = name,
                                fieldType = type
                            });
                            break;
                    }
                }

                if (string.IsNullOrEmpty(config.className))
                {
                    return false;
                }

                if (config.fileKeys.Count == 0)
                {
                    config.fileKeys.Add(config.className);
                }
                
                _dataConfigInfos.Add(config.className, config);                
            }

            return true;
        }

        /// <summary>
        /// 根据DataConfig文件生成空csv文件
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static bool CreateMoreCsvFile()
        {
            if(!ReadDataConfigFile(true))
            {
                Debug.Log("读取DataConfig文件失败");
                return false;
            }
            
            foreach (var kv  in _dataConfigInfos)
            {
                var curClassName = kv.Key;

                var classes = new Stack<string>();

                #region 将当前类所有祖先类都以此压入classes中
                
                classes.Push(curClassName);
                var temp = curClassName;
                while (!string.IsNullOrEmpty(temp))
                {
                    if (!_dataConfigInfos.ContainsKey(temp))
                    {
                        throw new Exception($"{DataConfigFileName}文件中未注册的类型：{temp}");
                    }
                    temp = _dataConfigInfos[temp].parentName;
                    if(!string.IsNullOrEmpty(temp))
                        classes.Push(temp);
                }
                

                #endregion

                
                var nameList=new List<string>();
                var typeList=new List<string>();

                #region 将当前类及其所有祖先类的所有字段和类型加入到nameList和typeList中去

                while (classes.Count > 0)
                {
                    var name = classes.Pop();
                    var config = _dataConfigInfos[name];
                    foreach (var _kv in config.fieldInfos)
                    {
                        nameList.Add(_kv.Value.fieldName);
                        typeList.Add(_kv.Value.fieldType);
                    }
                }                

                #endregion


                Assert.IsTrue(kv.Value.fileKeys.Count>0);
                foreach (var fileKey in kv.Value.fileKeys)
                {
                    #region 生成文件：typeName_fileKey

                    var fileName = curClassName + "_" + fileKey;
                                        
                    if (File.Exists(csvDirPath + "/" + fileName + ".csv"))
                    {
                        if(overriteOldFile)
                            File.Delete(csvDirPath + "/" + fileName + ".csv");
                        else
                        {
                            continue;
                        }
                    }
                
                    var stream = new StreamWriter(csvDirPath + "/" + fileName + ".csv");

                    for (var i = 0; i < nameList.Count; i++)
                    {
                        if(i==nameList.Count-1)
                            stream.Write(nameList[i]);
                        else 
                            stream.Write(nameList[i]+",");
                    }

                    stream.Write("\n");
                    for (var i = 0; i < typeList.Count; i++)
                    {
                        if(i==typeList.Count-1)
                            stream.Write(typeList[i]);
                        else 
                            stream.Write(typeList[i]+",");
                    }
                
                
                    stream.Dispose();
                    stream.Close();  

                    #endregion
                }
                
            }

            return true;
        }

        private static string GetIdName(string typeName)
        {
            Assert.IsTrue(_dataConfigInfos.ContainsKey(typeName));
            var info = _dataConfigInfos[typeName];
            while (!string.IsNullOrEmpty(info.parentName)
                   && _dataConfigInfos.ContainsKey(info.parentName))
            {
                info = _dataConfigInfos[info.parentName];
            }

            return info.fieldInfos.First().Value.fieldName;
        }

        /// <summary>
        /// 创建对应数据结构的类型定义
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="writePath"></param>
        /// <param name="nameSpace"></param>
        private static void CreatConfigFile(string filePath, string writePath,string nameSpace)
        {
            var targetPath = Application.dataPath + "/Resources/ClassFile";

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            string className = null;
            string fileKey = null;

            #region 获取className,fileKey并设置好targetPath
            
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var strs = fileName.Split('_');

                if (strs.Length != 2)
                {
                    throw new Exception($"异常文件名：{fileName}");
                }
                className = strs[0].Trim();
                fileKey = strs[1].Trim();
                targetPath += "/" + fileName + ".csv";
            }

            #endregion
            

            if (new FileInfo(targetPath).FullName != new FileInfo(filePath).FullName)
            {
                File.Copy(filePath, targetPath, true);
            }

            var sw = new StreamWriter(writePath + "/" + className + ".cs");

            var parentClass = _dataConfigInfos[className].parentName;

            if (string.IsNullOrEmpty(parentClass))
            {
                sw.Write("using ReadyGamerOne.Data;\n");
                parentClass = "CsvMgr";
            }
            
            sw.WriteLine("using UnityEngine;\nusing System.Collections;\n");
            var ns = string.IsNullOrEmpty(nameSpace) ? "DefaultNamespace" : nameSpace;
            sw.WriteLine("namespace "+ns+"\n" +
                         "{\n\t[System.Serializable]\n\tpublic class " + className + " : "+ parentClass);
            sw.WriteLine("\t{");

            var csr = new CsvStreamReader(filePath);
            
            sw.WriteLine("\t\tpublic const int "+className+"Count = "+(csr.RowCount-2)+";\n");

            var index = 0;
            foreach (var kv in _dataConfigInfos[className].fieldInfos)
            {
                var fieldName = kv.Value.fieldName;
                var fieldType = kv.Value.fieldType;
                
                if(index==0)
                    sw.WriteLine("\t\tpublic override string ID => "+GetIdName(className)+".ToString();\n");
                
                sw.WriteLine("\t\t" + "public " + fieldType + " " + fieldName + ";" + "");
                index++;
            }

            var toStringFunc = "\t\tpublic override string ToString()\n" +
                               "\t\t{\n" +
                               "\t\t\tvar ans=\"==《\t"+className+"\t》==\\n\" +\n";
            
            for (int colNum = 1; colNum < csr.ColCount + 1; colNum++)
            {
                string fieldName = csr[1, colNum];
                string fieldType = csr[2, colNum];
                
                toStringFunc += "\t\t\t\t\t" + "\"" + fieldName + "\" + \"\t\" + " + fieldName + "+\"\\n\" +\n";
            }

            toStringFunc = toStringFunc.Substring(0, toStringFunc.Length - "+\"\\n\" +\n".Length);
            
            toStringFunc += ";\n" +
                            "\t\t\treturn ans;\n" +
                            "\n\t\t}\n";
            
            sw.WriteLine(toStringFunc);
            
            sw.WriteLine("\t}\n" +
                         "}\n");

            sw.Flush();
            sw.Close();
        }
    }
#endif
    
}

