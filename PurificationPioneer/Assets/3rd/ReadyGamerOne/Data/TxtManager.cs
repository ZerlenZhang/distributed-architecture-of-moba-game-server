using System;
using System.Collections.Generic;
using System.IO;
using ReadyGamerOne.Const;
using UnityEngine;

namespace ReadyGamerOne.Data
{
    public static class TxtManager
    {
        public static char SplitChar => TxtSign.txtSplitChar;

        /// <summary>
        /// 记录下所有物品的工厂
        /// </summary>
        private static Dictionary<string, IDataFactory> factoryDic = new Dictionary<string, IDataFactory>();

        private static Stack<ITxtSerializable> handler = new Stack<ITxtSerializable>();


        /// <summary>
        /// 注册不同种类的工厂
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="factory"></param>
        public static void RegisterDataFactory<T>(string dataType)
            where T : class, ITxtSerializable, new()
        {
            if (!factoryDic.ContainsKey(dataType))
                factoryDic.Add(dataType, DataFactory<T>.Instance);
            else
                Debug.LogWarning("重复添加物品: " + dataType);
        }


        /// <summary>
        /// 读取有效行
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static string ReadUntilValue(StreamReader sr)
        {
            string line = null;
            do
            {
                line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("//"))
                    continue;
                line = line.Trim();
                break;
            } while (true);

            return line;
        }


        /// <summary>
        /// 根据路径从文件中加载数据
        /// </summary>
        /// <param name="streamingAssertPath"></param>
        /// <param name="action"></param>
        /// <typeparam name="DataType"></typeparam>
        public static void LoadDataFromFile<DataType>(string streamingAssertPath, Action<DataType> action)
            where DataType : class, ITxtSerializable
        {
            streamingAssertPath = Application.streamingAssetsPath + "\\" + streamingAssertPath;
            var sr = new StreamReader(streamingAssertPath);
            do
            {
                var item = TxtManager.LoadData(sr) as DataType;
                if (item == null)
                {
                    Debug.LogWarning("item is null");
                    break;
                }

                if (action != null)
                    action(item);
            } while (true);

            sr.Close();
        }

        /// <summary>
        /// 从文件流中解析数据
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static object LoadData(StreamReader sr)
        {
            bool backet = false;
            ITxtSerializable data = null;
            do
            {
                string line = sr.ReadLine();
                if (line == null)
                {
//					Debug.Log("line is null, break");
                    break;
                }

                line = line.Trim();

                //注释写法   //注释
                if (line.StartsWith("//") || line == "")
                    continue;

                if (line.StartsWith("End"))
                    break;

                //解析文件

                if (line.StartsWith("{")) //开始大括号
                {
                    backet = true;
//					Debug.Log(handler.Peek().Sign + " {");
                    handler.Peek().LoadTxt(sr);
                }
                else if (line.StartsWith("}")) //结束大括号
                {
                    backet = false;
                    data = handler.Peek();
//					Debug.Log(data.Sign + " }");
                    handler.Pop();
                    return data;
                }
                else
                {
                    if (backet == false)
                    {
                        var sign = line.Split(SplitChar)[0];
                        data = factoryDic[sign].CreateData(line);
                        handler.Push(data);
                    }
                }
            } while (true);


            return data;
        }
    }
}