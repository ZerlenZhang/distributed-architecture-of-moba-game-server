using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ReadyGamerOne.Data
{
	public abstract class CsvMgr:IHasStringId
	{
		static Dictionary<string,Dictionary<string,CsvMgr>> dataDic = new Dictionary<string, Dictionary<string, CsvMgr>> ();

		/// <summary>
		/// 获取某个种类随机一个数据
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomData<T>(string filePath = null)
			where T:CsvMgr
		{
			var setT = typeof(T);
			if (filePath == null) {
				filePath =setT.Name;
			}

			if (!dataDic.ContainsKey(filePath)) {
				ReadConfigData<T>(filePath);
			}
			
			var objDic = dataDic[filePath];
			var keyList = objDic.Keys.ToList();
			var randomIndex = Random.Range(0, keyList.Count - 1);
			return objDic[keyList[randomIndex]] as T;
		}
		
		/// <summary>
		/// 根据文件路径和数据键获取一条数据
		/// </summary>
		/// <param name="fileLoadPath"></param>
		/// <param name="dataKey"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static CsvMgr GetData(string fileLoadPath, string dataKey)
		{
			if (!dataDic.ContainsKey(fileLoadPath))
			{
				var type = Type.GetType(fileLoadPath);
				if(null==type)
					throw new Exception("CsvMgr中没有注册这个类型:	"+fileLoadPath);

				ReadConfigData(type);
			}

			var objDic = dataDic[fileLoadPath];
			if(!objDic.ContainsKey(dataKey))
				throw new Exception("CsvMgr注册了这个类，但表中没有这个ID:	"+dataKey);
			return objDic[dataKey];
		}
		
		/// <summary>
		/// 跟据一个Type和Key获取一个数据
		/// </summary>
		/// <param name="dataClassType"></param>
		/// <param name="dataKey"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static CsvMgr GetData(Type dataClassType, string dataKey)
		{
			if (null == dataClassType)
				throw new Exception("数据类为空");
			if (!dataDic.ContainsKey(dataClassType.Name))
			{
				ReadConfigData(dataClassType);
			}

			var objDic = dataDic[dataClassType.Name];
			if(!objDic.ContainsKey(dataKey))
				throw new Exception("CsvMgr注册了这个类，但表中没有这个ID:	"+dataKey);
			return objDic[dataKey];
		}
		/// <summary>
		/// 根据T，tab 获取一条数据
		/// </summary>
		/// <param name="key"></param>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static T GetData<T> (string key, string filePath = null) where T:CsvMgr
		{
			var setT = typeof(T);
			if (filePath == null) {
				filePath =setT.Name;
			}

			if (!dataDic.ContainsKey(filePath)) {
				ReadConfigData<T>(filePath);
			}
			
			
			var objDic = dataDic[filePath];
			if (!objDic.ContainsKey (key)) {
				throw new Exception("CsvMgr注册了这个类，但表中没有这个ID:	"+key);
			}
			return (T)(objDic [key]);
		}

		/// <summary>
		/// 根据T获取所有类型数据
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> GetDatas<T> (string filePath = null) where T:CsvMgr
		{
			var returnList = new List<T> ();
			var setT = typeof(T);
			if (filePath == null) {
				filePath = setT.Name;
			}

			if (!dataDic.ContainsKey(filePath))
			{
				ReadConfigData<T> (filePath);
			}
			var objDic = dataDic[filePath];
			foreach (var kvp in objDic) {
				returnList.Add ((T)(kvp.Value));
			}
			return returnList;
		}

		static void ReadConfigData(Type type, string filePath = null)
		{
			var fileLoadPath = filePath;
			if (string.IsNullOrEmpty(fileLoadPath)||fileLoadPath==type.Name)
			{
				fileLoadPath = type.Name;
			}
			
			string getString = ResourceMgr.GetAsset<TextAsset>(fileLoadPath,OriginBundleKey.File).text;

			var csr = new CsvReaderByString (getString);

			var dataRow = new Dictionary<string, CsvMgr> ();
				
			FieldInfo[] fieldInfos = new FieldInfo[csr.ColCount];
			for (int colNum=1; colNum<csr.ColCount+1; colNum++) {
				fieldInfos [colNum - 1] = type.GetField (csr [1, colNum]);
			}

			//每行都是一条数据，所以循环行数次
			for (int i=3; i<csr.RowCount+1; i++) {
				
				var dataObj = Activator.CreateInstance(type);
				
				//逐个判断当前行每一列域类型加入字典
				for (int j=0; j<fieldInfos.Length; j++) {
					string fieldValue = csr [i, j + 1];
					
					//试探可能的类型，将数据填充到数据项dataItem中
					object dataItem = new object ();
					switch (fieldInfos [j].FieldType.ToString ()) {
						case "System.Single":
							dataItem = float.Parse(fieldValue);
							break;
						case "System.Int32":
							dataItem = int.Parse (fieldValue);
							break;
						case "System.Int64":
							dataItem = long.Parse (fieldValue);
							break;
						case "System.String":
							dataItem = fieldValue;
							break;
						default:
							Debug.LogWarning("error data type");
							break;
					}
					
					fieldInfos [j].SetValue (dataObj, dataItem);
					
					//如果是第一列，这一列被视为主键
					if (j==0) {
						dataRow.Add (dataItem.ToString (), (CsvMgr)dataObj);
					}
				}
			}
			
			dataDic.Add(fileLoadPath, dataRow);    //可以作为参数
		}

		static void ReadConfigData<T> (string filePath = null) where T:CsvMgr
		{
			ReadConfigData(typeof(T), filePath);
		}

		public abstract string ID { get; }
	}
}
