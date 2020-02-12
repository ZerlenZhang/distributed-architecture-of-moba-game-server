using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class FileUtil
    {
        #region 目录操作

        /// <summary>
        /// 当前可执行文件路径
        /// </summary>
        public static string CurrentRunDirectory => Environment.CurrentDirectory;
        /// <summary>
        /// 获取父级目录
        /// </summary>
        /// <param CharacterName="pathNum"></param>
        /// <returns></returns>
        public static string GetRunDirectoryInParentPath(int pathNum=3)
        {
            //    /Server/Server/bin/Debug/server.exe
            DirectoryInfo pathInfo = Directory.GetParent(CurrentRunDirectory);
            while (pathNum > 0 && pathInfo.Parent != null)
            {
                var info = pathInfo.Parent;
                pathNum--;
            }
            //返回一个完整的文件夹路径
            return pathInfo.FullName;
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <returns></returns>
        public static string CreateFolder(string path)
        {
            //如果目录存在则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;



        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="foldName"></param>
        /// <returns></returns>
        public static string CreateFolder(string path,string foldName)
        {
            var fullPath = path + "//" + foldName;
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        /// <summary>
        /// 获取目录名
        /// </summary>
        /// <param name="dirFullPath"></param>
        /// <returns></returns>
        public static string GetDirName(string dirFullPath)
        {
            return new DirectoryInfo(dirFullPath).Name;
        }

        /// <summary>
        /// 获取父目录名
        /// </summary>
        /// <param name="dirFullPath"></param>
        /// <returns></returns>
        public static string GetParentDirName(string dirFullPath)
        {
            return Directory.GetParent(dirFullPath).Name;
        }        

        #endregion
        



        /// <summary>
        /// 创建文件（初始化文本）
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <param CharacterName="info"></param>
        public static void CreateFile(string path,string fileName,string info=null)
        {
            CreateFolder(path);
            var fileInfo = new FileInfo(path + "//" + fileName);
            if (fileInfo.Exists)
                fileInfo.Delete();
            var streamWriter = fileInfo.CreateText();

            if(!string.IsNullOrEmpty(info))
                streamWriter.WriteLine(info);

            streamWriter.Close();
            streamWriter.Dispose();
        }
        public static void CreateFile(string fullPath, byte[] data)
        {
            //Create the Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            try
            {
                File.WriteAllBytes(fullPath, data);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Save Data to: " + fullPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }
        
        /// <summary>
        /// 向文件中添加信息
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <param CharacterName="info"></param>
        public static void AddInfoToFile(string path,string fileName,string info)
        {
            CreateFolder(path);

            var fileInfo = new FileInfo(path + "//" + fileName);

            var streamWriter = !fileInfo.Exists ? fileInfo.CreateText() : fileInfo.AppendText();

            streamWriter.WriteLine(info);

            streamWriter.Close();

            streamWriter.Dispose();

        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <returns></returns>
        public static string LoadFile(string path,string fileName)
        {
            if (!FileExist(path, fileName))
                return null;

            if (!path.EndsWith("/"))
                path = path + "/";
            
            var streamReader = new StreamReader(path + fileName);

            var arr = new ArrayList();

            while (true)
            {
                var line = streamReader.ReadLine();
                if (line == null)
                    break;
                arr.Add(line);
            }
            streamReader.Close();
            streamReader.Dispose();
            string ans = "";
            foreach (var str in arr)
                ans += str;
            return ans;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param CharacterName="path"></param>
        /// <param CharacterName="fileName"></param>
        /// <returns></returns>
        public static bool FileExist(string path,string fileName)
        {
            if (!Directory.Exists(path))
                return false;
            if (!File.Exists(path + "//" + fileName))
                return false;
            return true;
        }

        /// <summary>
        /// 根据字典生成常量类
        /// </summary>
        /// <param name="className">常量类名</param>
        /// <param name="generateDir">生成目录</param>
        /// <param name="nameSpace">常量类命名空间</param>
        /// <param name="content">常量名和值组成的字典</param>
        public static void CreateConstClassByDictionary(string className, string generateDir, string nameSpace,
            Dictionary<string, string> content)
        {
            var constClassFullPath = generateDir + "/"+className+".cs";
            if (!Directory.Exists(generateDir))
                Directory.CreateDirectory(generateDir);
            if(File.Exists(constClassFullPath))
                File.Delete(constClassFullPath);
            var stream = new StreamWriter(constClassFullPath);
            
            var ns = string.IsNullOrEmpty(nameSpace) ? "DefaultNameSpace" : nameSpace;
            
            stream.Write("namespace "+ns+"\n" +
                         "{\n" +
                         "\tpublic partial class "+className+ "\n" +
                         "\t{\n");

            foreach (var kv in content)
            {
                stream.Write("\t\tpublic const string "+kv.Key+" = @\""+kv.Value+"\";\n");                
            }
            
            stream.Write("\t}\n" +
                         "}\n");
            stream.Flush();
            stream.Dispose();
            stream.Close();
        }

        public static string FileNameToVarName(string fileName)
        {
            string ans = "";
            ans = fileName.Replace(" ", "");
            ans = ans.Replace("`", "");
            return ans;
        }
        
        /// <summary>
        /// 创建一个常量类，常量类里存放的是指定目录下的文件名，用同名变量保存
        /// </summary>
        /// <param name="className">常量文件名</param>
        /// <param name="generateDir">生成目录</param>
        /// <param name="dirWhichContainsFiles">包含目标文件的目录</param>
        /// <param name="nameSpace">常量类的命名空间</param>
        /// <param name="onOperateFile">操作文件的委托</param>
        /// <param name="deepSearch">是否递归搜索</param>
        public static void ReCreateFileNameConstClassFromDir(string className, string generateDir,string dirWhichContainsFiles, string nameSpace,Action<FileInfo,StreamWriter> onOperateFile=null,bool deepSearch=false)
        {
            var classFilePath = generateDir + "/"+className+".cs";
            if(File.Exists(classFilePath))
                File.Delete(classFilePath);
            var stream = new StreamWriter(classFilePath);
            
            var ns = string.IsNullOrEmpty(nameSpace) ? "DefaultNameSpace" : nameSpace;
            
            stream.Write("namespace "+ns+"\n" +
                         "{\n" +
                         "\tpublic partial class "+className+ "\n" +
                         "\t{\n");
            
            if(null==onOperateFile)
                SearchDirectory(dirWhichContainsFiles, fileInfo =>
                {
                    if (!fileInfo.Name.EndsWith(".meta"))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        var name = FileNameToVarName(fileName);
                        stream.Write("\t\tpublic const string "+name+" = @\""+fileName+"\";\n");
                    }
                },deepSearch);
            else 
                SearchDirectory(dirWhichContainsFiles,fileInfo=>onOperateFile(fileInfo,stream),deepSearch);


            stream.Write("\t}\n" +
                         "}\n");
            stream.Flush();
            stream.Dispose();
            stream.Close();
        }

        /// <summary>
        /// 遍历目录，对其中文件执行操作
        /// </summary>
        /// <param name="startDirPath">起始目录</param>
        /// <param name="onOperateFile">操作文件的函数</param>
        /// <param name="deepSearch">是否递归搜索，这样会进入每一个子目录里面递归搜索</param>
        /// <param name="onoperateDir">操作目录的函数</param>
        /// <exception cref="Exception"></exception>
        public static void SearchDirectory(
            string startDirPath, 
            Action<FileInfo> onOperateFile, 
            bool deepSearch = false,
            Func<DirectoryInfo,bool> onoperateDir=null)
        {
            if (!Directory.Exists(startDirPath))
                throw new Exception("起始文件夹不存在");

            bool ifSearch = true;
            if(onoperateDir!=null && Directory.Exists(startDirPath))
                ifSearch = onoperateDir.Invoke(new DirectoryInfo(startDirPath));



            foreach (var info in Directory.GetFileSystemEntries(startDirPath))
            {
                var fullPath = info;
                if (Directory.Exists(fullPath))
                {           
                    ifSearch = ifSearch && deepSearch;
                    if (!ifSearch)
                        continue;
                    SearchDirectory(fullPath,onOperateFile,deepSearch,onoperateDir);
                }
                else //否则就是文件
                {
                    onOperateFile?.Invoke(new FileInfo(fullPath));
                }
            }
        }


        /// <summary>
        /// 创建空的类文件
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="generateDir">生成目录</param>
        /// <param name="parentClass">父类名</param>
        /// <param name="helpTips">注释</param>
        /// <param name="fileContent">类内容</param>
        /// <param name="autoOverwrite">是否自动覆盖重写</param>
        public static void CreateClassFile(string className, string nameSpace, string generateDir,
            string parentClass = null, string helpTips = null, string fileContent = null,
            bool autoOverwrite = false, bool partical = false, string usingStatements = null,bool isAbstract=false,
            string otherClassBody=null)
        {
            CreateFolder(generateDir);
            var fullPath = generateDir + "/" + className + ".cs";
            if (File.Exists(fullPath))
            {
                if (autoOverwrite)
                    File.Delete(fullPath);
                else
                    return;
            }
            
            var stream = File.CreateText(fullPath);

            if (string.IsNullOrEmpty(usingStatements) == false)
                stream.Write(usingStatements);
            
            stream.Write("namespace " + nameSpace + "\n" +
                         "{\n");

            if (!string.IsNullOrEmpty(helpTips))
                stream.Write("\t/// <summary>\n" +
                             "\t/// " + helpTips + "\n" +
                             "\t/// </summary>\n");


            stream.Write("\tpublic ");
            if (partical)
                stream.Write("partial ");
            if (isAbstract)
                stream.Write("abstract ");

            stream.Write("class " + className);
            
            if (!string.IsNullOrEmpty(parentClass))
                stream.Write(" : " + parentClass);
            stream.Write("\n" +
                         "\t{\n");
            if (!string.IsNullOrEmpty(fileContent))
                stream.Write(fileContent);

            stream.Write("\t}\n");
            if (!string.IsNullOrEmpty(otherClassBody))
                stream.Write(otherClassBody);
            stream.Write("}\n");
            
            stream.Dispose();
            stream.Close();
        }
        

    }
}