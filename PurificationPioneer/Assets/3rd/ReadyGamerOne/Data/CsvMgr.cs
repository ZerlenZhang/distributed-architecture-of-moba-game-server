using System;
using System.Collections.Generic;
using System.Linq;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace ReadyGamerOne.Data
{
	public abstract class CsvMgr:IHasStringId
	{
		public class FieldInfo
		{
			public string fieldName;
			public string fieldType;
		}
		public class DataConfigInfo
		{
			public string className;
			public string parentName;
			public Dictionary<string,FieldInfo> fieldInfos=new Dictionary<string,FieldInfo>();
			public List<string> childrenNames=new List<string>();
			public List<string> fileKeys=new List<string>();
		}

		#region Private

		private static Dictionary<string, DataConfigInfo> _dataConfigInfos;
		
		/// <summary>
		/// 数据字典 Type => FileKey => data
		/// </summary>
		private static Dictionary<string,
			Dictionary<string,Dictionary<string,CsvMgr>>> allDataDic=new Dictionary<string, Dictionary<string, Dictionary<string, CsvMgr>>>();

		/// <summary>
		/// 安全插入数据字典
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="dataDic"></param>
		/// <param name="fileKey"></param>
		private static void SafeInsertDataDic(string typeName, Dictionary<string, CsvMgr> dataDic,
			string fileKey)
		{
			Assert.IsNotNull(dataDic);
			Assert.IsFalse(string.IsNullOrEmpty(typeName));
			Assert.IsFalse(string.IsNullOrEmpty(fileKey));
			Assert.IsTrue(_dataConfigInfos.ContainsKey(typeName));

			if (!allDataDic.ContainsKey(typeName))
			{
				allDataDic.Add(typeName, new Dictionary<string, Dictionary<string, CsvMgr>>());
			}

			var fileKeyDic = allDataDic[typeName];
			if (!fileKeyDic.ContainsKey(fileKey))
			{
				fileKeyDic.Add(fileKey, new Dictionary<string, CsvMgr>());
			}

			var targetDataDic = fileKeyDic[fileKey];
			foreach (var data in dataDic)
			{
				if (targetDataDic.ContainsKey(data.Key))
				{
					Debug.LogError($"{typeName}:{fileKey}重复ID：{data.Key}");
					continue;
				}

				targetDataDic.Add(data.Key, data.Value);
			}
		}

		/// <summary>
		/// 不考虑继承，安全查询一条数据，fileName为Null默认查询所有字典,fileName无效会报警告并查询所有字典
		/// </summary>
		/// <param name="dataId"></param>
		/// <param name="typeName"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static CsvMgr SearchDataInternal(string dataId, Type type, string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(dataId));
			Assert.IsNotNull(type);
			Assert.IsTrue(_dataConfigInfos.ContainsKey(type.Name));
			
			var typeName = type.Name;

			//typeName库中有没有:
			if (!allDataDic.ContainsKey(typeName))
			{// 没有type就加载
				foreach (var fileKey in _dataConfigInfos[typeName].fileKeys)
				{
					// Debug.Log($"type:{typeName}, fileKey:{fileKey}");
					LoadConfigData(type,typeName+"_"+fileKey);
				}
			}
			
			// Debug.Log(2);
			Assert.IsTrue(allDataDic.ContainsKey(typeName));

			var fileDics = allDataDic[typeName];
			var warning = false;
			var searchall = false;
			if (string.IsNullOrEmpty(fileName))
			{
				searchall = true;
			}else if (!fileDics.ContainsKey(fileName))
			{
				searchall = true;
				warning = true;
			}

			if (searchall)
			{// 寻找所有dataDic
				foreach (var fileKey_dataDic in fileDics)
				{
					// Debug.Log($"try search {fileKey_dataDic.Key}, dataId: {dataId}");
					var dataDic = fileKey_dataDic.Value;
					if (dataDic.Count == 0)
					{
						if (warning)
						{
							Debug.LogWarning($"dataDic is empty: TypeName{typeName}, ItemId{dataId}, FileName{fileName}，已查询所有字典代替");
						}
					}
					else if (dataDic.ContainsKey(dataId))
					{
						if (warning)
						{
							Debug.LogWarning($"无效FileKey:{fileName}，已查询所有字典代替");
						}

						return dataDic[dataId];						
					}
				}
				Debug.Log($"Failed from search all, dataId:{dataId}");
			}
			else
			{
				//寻找指定FileKeyDic
				var dataDic = fileDics[fileName];
				if (dataDic.ContainsKey(dataId))
				{
					return dataDic[dataId];
				}
			}

			Debug.LogError($"???,type:{type.Name},fileName:{fileName}");
			return null;
		}		
		
		/// <summary>
		/// 不考虑继承，安全查询所有typename的数据，fileName为Null默认查询所有字典,fileName无效会报警告并查询所有字典
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private static Dictionary<string, CsvMgr> GetDatasInternal(Type dataType, string fileName)
		{
			Assert.IsNotNull(dataType);
			Assert.IsTrue(_dataConfigInfos.ContainsKey(dataType.Name));

			var typeName = dataType.Name;

			if (!allDataDic.ContainsKey(dataType.Name))
			{
				foreach (var fileKey in _dataConfigInfos[typeName].fileKeys)
				{
					LoadConfigData(dataType,typeName+"_"+fileKey);
				}
			}

			Assert.IsTrue(allDataDic.ContainsKey(dataType.Name));
			
			var ans = new Dictionary<string,CsvMgr>();
			
			var fileDics = allDataDic[dataType.Name];
			var searchall = false;
			if (string.IsNullOrEmpty(fileName))
			{
				searchall = true;
			}else if (!fileDics.ContainsKey(fileName))
			{
				searchall = true;
				Debug.LogWarning($"无效FileKey:{fileName}，已查询所有字典代替");
			}

			if (searchall)
			{
				foreach (var fileKey_dataDic in fileDics)
				{
					foreach (var id_data in fileKey_dataDic.Value)
					{
						if (id_data.Value != null)
						{
							ans.Add(id_data.Key, id_data.Value);
						}
					}
				}
			}
			else
			{
				foreach (var id_data in fileDics[fileName])
				{
					if (id_data.Value != null)
					{
						ans.Add(id_data.Key, id_data.Value);
					}
				}
			}


			return ans;
		}


		/// <summary>
		/// 根据typeName获取本身及其子类所有需要加载的
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static List<string> GetAllFileNameWithTypeName(string typeName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(typeName));
			
			if(null==_dataConfigInfos)
				LoadRootConfigData();
			if(!_dataConfigInfos.ContainsKey(typeName))
				throw new Exception($"未注册的类型:{typeName}");
			var ans=new List<string>();
			//添加自身
			foreach (var fileKey in _dataConfigInfos[typeName].fileKeys)
			{
				ans.Add(typeName+"_"+fileKey);
			}
			//添加子类
			foreach (var typeName_info in _dataConfigInfos)
			{
				if (typeName_info.Value.parentName == typeName)
				{
					ans.AddRange(GetAllFileNameWithTypeName(typeName_info.Key));
				}
			}

			return ans;
		}
		
		
		/// <summary>
		/// 加载数据，fileKey不合法的时候默认取type.Name
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filePath"></param>
		private static void LoadConfigData(Type type, string fileName)
		{
			Assert.IsNotNull(type);
			Assert.IsNotNull(_dataConfigInfos);
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			
			CsvReaderByString csr = null;

			#region 加载csr并初始化fileKey
			
			//fileKey不合法的时候默认取type.Name
			
			var asset = ResourceMgr.GetAsset<TextAsset>(fileName, OriginBundleKey.File);
			
			if(asset==null)
				throw new Exception($"加载数据文件失败,typeKey:{type.Name},fileName:{fileName}");
			
			csr = new CsvReaderByString (asset.text);				

			#endregion
			
			System.Reflection.FieldInfo[] fieldInfos = null;

			#region 根据type初始化fieldInfos

			
			fieldInfos = new System.Reflection.FieldInfo[csr.ColCount];
			for (var colNum=1; colNum<csr.ColCount+1; colNum++) {
				var info = type.GetField (csr [1, colNum]);

				if (info == null)
				{
					throw new Exception($"GetField failed:如果使用了多态，请尽量使用具体类型代替\ntype:{type.Name}\nfileKey:{fileName}\nfieldName:{csr[1,colNum]}");
				}
				
				fieldInfos[colNum - 1] = info;
			}	

			#endregion
			

			var dataRow = new Dictionary<string, CsvMgr> ();

			//每行都是一条数据，所以循环行数次
			for (var i=2; i<csr.RowCount+1; i++) {
				
				var dataObj = Activator.CreateInstance(type);
				
				//逐个判断当前行每一列域类型加入字典
				for (var j=0; j<fieldInfos.Length; j++) {
					var fieldValue = csr [i, j + 1];
					
					//试探可能的类型，将数据填充到数据项dataItem中
					var dataItem = new object ();
					try
					{
						switch (fieldInfos [j].FieldType.ToString ()) {
							case "System.Single":
								dataItem = string.IsNullOrEmpty(fieldValue) ? default(float) : float.Parse(fieldValue);
								break;
							case "System.Int32":
								dataItem = string.IsNullOrEmpty(fieldValue) ? default(int) : int.Parse (fieldValue);
								break;
							case "System.Int64":
								dataItem = string.IsNullOrEmpty(fieldValue) ? default(long) : long.Parse (fieldValue);
								break;
							case "System.String":
								dataItem = string.IsNullOrEmpty(fieldValue) ? default(string) : fieldValue;
								break;
							default:
								throw new Exception($"意料之外的字段类型:{fieldInfos [j].FieldType}\ntype:{type.Name}\nfileKey:{fileName}");
						}
					}
					catch (FormatException e)
					{
						throw new Exception($"{e.Message}\ninputString:{fieldValue}\ntype:{type.Name}\nfileKey:{fileName}\npos:[{i},{j}]");
					}
					

					
					fieldInfos [j].SetValue (dataObj, dataItem);
					
					//如果是第一列，这一列被视为主键
					if (j==0) {
//						Debug.Log(dataObj);
						dataRow.Add (dataItem.ToString (), (CsvMgr)dataObj);
					}
				}
				
				// Debug.Log($"$insert data {dataObj}");
			}

			if (dataRow.Count == 0)
			{
				Debug.LogWarning($"itemDataRow is empty: TypeName{type.Name}, fileName:{fileName}");
			}
			
			//debug
			// foreach (var kv in dataRow)
			// {
			// 	Debug.Log($"success get data: {kv.Value}");
			// }

			SafeInsertDataDic(
				type.Name, 
				dataRow, 
				fileName);
		}

		/// <summary>
		/// 读取数据结构信息
		/// </summary>
		private static void LoadRootConfigData()
		{
			//避免重复初始化
			if (_dataConfigInfos != null)
				return;
			
			_dataConfigInfos=new Dictionary<string, DataConfigInfo>();
			var getString = ResourceMgr.GetAsset<TextAsset>("DataConfig",OriginBundleKey.File).text;

			var csr = new CsvReaderByString (getString);
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
							if (!string.IsNullOrEmpty(name))
							{
								config.parentName = name;
								_dataConfigInfos[config.parentName]
									.childrenNames.Add(config.className);
							}
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
					throw new Exception("ClassName is null ???");
				}
				if (config.fileKeys.Count == 0)
				{
					config.fileKeys.Add(config.className);
				}

				_dataConfigInfos.Add(config.className, config);                
			}
		}		
		
		#endregion

		
		/// <summary>
		/// 考虑继承，安全查询数据，fileKey为Null默认查询所有字典,fileKey无效会报警告并查询所有字典
		/// </summary>
		/// <param name="dataId"></param>
		/// <param name="typeName"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static CsvMgr GetData(string dataId, Type dataType, string fileName = null)
		{
			dataId = dataId.Trim();
			
			if(_dataConfigInfos==null)
				LoadRootConfigData();
			
			Assert.IsFalse(string.IsNullOrEmpty(dataId));
			Assert.IsNotNull(dataType);
			
			//如果数据结构表中没有，那么必然没有
			if (!_dataConfigInfos.ContainsKey(dataType.Name))
			{
				Debug.LogWarning($"未注册的数据类型：{dataType.Name}");
				return null;
			}
			
			// Debug.Log(1);
			var ans = SearchDataInternal(dataId, dataType, fileName);
			if (ans != null)
				return ans;

			//查找子类型
			var nameSpace = dataType.Namespace;
			foreach (var typeName_info in _dataConfigInfos)
			{
				var info = typeName_info.Value;
				if (info.parentName != dataType.Name) 
					continue;
				
				
				var typeName = nameSpace + "." + info.className;
				var childType = Type.GetType(typeName);
				if (childType == null)
				{
					Debug.LogError($"错误类型:{typeName},dataId:{dataId}");
					continue;
				}
				if (!_dataConfigInfos.ContainsKey(childType.Name))
				{
					Debug.LogError($"未注册的类型：{typeName},dataId:{dataId}");
					continue;
				}
					
				ans = SearchDataInternal(dataId, childType, fileName);
				
				if (ans != null)
					return ans;
			}

			throw new Exception($"遍历所有子类型也没有找到这个id:{dataId},typeName:{dataType.Name}");
		}

		/// <summary>
		/// 考虑继承，安全查询数据返回T，fileKey为Null默认查询所有字典,fileKey无效会报警告并查询所有字典
		/// </summary>
		/// <param name="dataId"></param>
		/// <param name="fileName"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetData<T>(string dataId, string fileName = null)
			where T:CsvMgr
		{
			return GetData(dataId, typeof(T), fileName) as T;
		}
		
		/// <summary>
		/// 获取某个种类随机一个数据
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetRandomData<T>(string fileName = null)
			where T:CsvMgr
		{
			//如果没有读取数据结构表，就先读取
			if(_dataConfigInfos==null)
				LoadRootConfigData();

			var typeName = typeof(T).Name;
			
			
			//如果数据结构表中没有，那么必然没有
			if (!_dataConfigInfos.ContainsKey(typeName))
			{
				Debug.LogError($"未注册的数据类型：{typeName}");
				return null;
			}

			//如果数据库中没有加载，就加载
			if (!allDataDic.ContainsKey(typeName))
			{
				foreach (var fileKey in _dataConfigInfos[typeName].fileKeys)
				{
					LoadConfigData(typeof(T),typeName+"_"+fileKey);
				}
			}
			
			var fileDic = allDataDic[typeName];
			
			var all = 0;
			var counts = new List<Dictionary<string,CsvMgr>>();
			
			
			if (fileName==null)
			{//	遍历本类型所有
				foreach (var fileKey_dataDic in fileDic)
				{
					all += fileKey_dataDic.Value.Count;
					counts.Add(fileKey_dataDic.Value);
				}
			}
			else if (string.IsNullOrEmpty(fileName))
			{// fileName=""
				Debug.LogWarning("fileName is empty, has search all");
				foreach (var fileKey_dataDic in fileDic)
				{
					all += fileKey_dataDic.Value.Count;
					counts.Add(fileKey_dataDic.Value);
				}
			}
			else
			{
				if (!fileDic.ContainsKey(fileName))
				{
					Debug.Log("Load from random");
					LoadConfigData(typeof(T),fileName);
					
					if(!fileDic.ContainsKey(fileName))
						throw new Exception($"Type:{typeName} don't have this fileName:{fileName}");
				}
				
				all += fileDic[fileName].Count;
				counts.Add(fileDic[fileName]);
			}

			var random = Random.Range(0, all);
			var temp = 0;
			foreach (var dataDic in counts)
			{
				if (random >= temp && random < temp + dataDic.Count)
				{//	找到目标
					return dataDic.Values.ToArray()[random-temp] as T;
				}

				temp += dataDic.Count;
			}

			throw new Exception("查找越界");
		}


		/// <summary>
		/// 根据typeName获取所有类型数据，如果fileKey为null，就查询所有，fileKey无效会警告
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		public static Dictionary<string,CsvMgr> GetDatas(Type dataType, string fileKey = null)
		{
			if(_dataConfigInfos==null)
				LoadRootConfigData();
			
			var ans = new Dictionary<string, CsvMgr>();
			
			if (!_dataConfigInfos.ContainsKey(dataType.Name))
				return ans;


			//加入当前类型
			var tempAns = GetDatasInternal(dataType, fileKey);
			if (tempAns != null)
			{
				foreach (var id_data in tempAns)
				{
					ans.Add(id_data.Key, id_data.Value);
				}
			}
			
			//加入子类型
			var nameSpace = dataType.Namespace;
			foreach (var typeName_info in _dataConfigInfos)
			{
				var info = typeName_info.Value;
				if (info.parentName == dataType.Name)
				{
					var childType = Type.GetType(nameSpace + info.className);
					if (childType == null)
					{
						Debug.LogError("错误类型:"+nameSpace + info.className);
						continue;
					}
					if (!_dataConfigInfos.ContainsKey(childType.Name))
					{
						Debug.LogError("未注册的类型："+nameSpace + info.className);
						continue;
					}
					
					//加入子类型
					tempAns = GetDatasInternal(childType, fileKey);
					if (tempAns != null)
					{
						foreach (var id_data in tempAns)
						{
							ans.Add(id_data.Key, id_data.Value);
						}
					}
					
				}
			}

			return ans;
		}

		/// <summary>
		/// 根据T获取所有类型数据，如果fileKey为null，就查询所有，fileKey无效会警告
		/// </summary>
		/// <param name="filePath"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Dictionary<string,T> GetDatas<T> (string fileKey = null) 
			where T:CsvMgr
		{
			var tempAns = GetDatas(typeof(T), fileKey);
			
			var ans = new Dictionary<string, T>();
			foreach (var id_data in tempAns)
			{
				var data = id_data.Value as T;
				if (data != null)
					ans.Add(id_data.Key, data);
			}

			return ans;
		}
		
		public abstract string ID { get; }
	}
}
