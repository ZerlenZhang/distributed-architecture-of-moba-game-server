using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;


namespace ReadyGamerOne.Data
{
	public class CsvStreamReader
	{
		private  ArrayList  rowAL;       //行链表,CSV文件的每一行就是一个链
		private  string  fileName;       //文件名
	
		private  Encoding encoding;       //编码
	
		/// <summary>
		///
		/// </summary>
		/// <param name="fileName">文件名,包括文件路径</param>
		public CsvStreamReader(string fileName)
		{
			this.rowAL = new ArrayList();
			this.fileName = fileName;
			this.encoding = Encoding.Default;
			LoadCsvFile();
		}
	
		/// <summary>
		///
		/// </summary>
		/// <param name="fileName">文件名,包括文件路径</param>
		/// <param name="encoding">文件编码</param>
		public CsvStreamReader(string fileName,Encoding encoding)
		{
			this.rowAL = new ArrayList();  
			this.fileName = fileName;
			this.encoding = encoding;
			LoadCsvFile();
		}
	
		/// <summary>
		/// 文件名,包括文件路径
		/// </summary>
		public string FileName
		{
			set
			{
				this.fileName = value;
				LoadCsvFile();
			}
		}
	
		/// <summary>
		/// 文件编码
		/// </summary>
	
		public Encoding FileEncoding
		{
			set
			{
				this.encoding = value;
			}
		}
	
		/// <summary>
		/// 获取行数
		/// </summary>
		public int RowCount
		{
			get
			{
				return this.rowAL.Count;
			}
		}
	
		/// <summary>
		/// 获取列数
		/// </summary>
		public int ColCount
		{
			get
			{
				int maxCol;
			
				maxCol = 0;
				for (int i = 0;i<this.rowAL.Count;i++)
				{
					ArrayList colAL = (ArrayList) this.rowAL[i];    //rowAL中的某个元素依然是ArrayList类型

					maxCol = (maxCol > colAL.Count)?maxCol:colAL.Count;     //取最大值
				}
			
				return maxCol;
			}
		}
	
	
		/// <summary>
		/// 获取某行某列的数据
	
		/// row:行,row = 1代表第一行
	
		/// col:列,col = 1代表第一列  
		/// </summary>
		public string this[int row,int col]
		{
			get
			{   
				//数据有效性验证
			
				CheckRowValid(row);
				CheckColValid(col);
				ArrayList colAL = (ArrayList) this.rowAL[row-1];
			
				//如果请求列数据大于当前行的列时,返回空值
			
				if (colAL.Count < col)
				{
					return "";
				}
			
				return colAL[col-1].ToString();    
			}
		}
	
	
		/// <summary>
		/// 根据最小行，最大行，最小列，最大列，来生成一个DataTable类型的数据
	
		/// 行等于1代表第一行
	
		/// <summary>
		/// 检查行数是否是有效的
	
		/// </summary>
		/// <param name="col"></param>  
		private void CheckRowValid(int row)
		{
			if (row <= 0)
			{
				throw new Exception("行数不能小于0");    
			} 
			if (row > RowCount)
			{
				throw new Exception("没有当前行的数据");   
			}  
		}
	
		/// <summary>
		/// 检查最大行数是否是有效的
	
		/// </summary>
		/// <param name="col"></param>  
		private void CheckMaxRowValid(int maxRow)
		{
			if (maxRow <= 0 && maxRow != -1)
			{
				throw new Exception("行数不能等于0或小于-1");    
			} 
			if (maxRow > RowCount)
			{
				throw new Exception("没有当前行的数据");   
			}  
		}
	
		/// <summary>
		/// 检查列数是否是有效的
	
		/// </summary>
		/// <param name="col"></param>  
		private void CheckColValid(int col)
		{
			if (col <= 0)
			{
				throw new Exception("列数不能小于0");    
			} 
			if (col > ColCount)
			{
				throw new Exception("没有当前列的数据");   
			}
		}
	
		/// <summary>
		/// 检查检查最大列数是否是有效的
	
		/// </summary>
		/// <param name="col"></param>  
		private void CheckMaxColValid(int maxCol)
		{
			if (maxCol <= 0 && maxCol != -1)
			{
				throw new Exception("列数不能等于0或小于-1");    
			} 
			if (maxCol > ColCount)
			{
				throw new Exception("没有当前列的数据");   
			}
		}

		/// <summary>
		/// 载入CSV文件
		/// </summary>
		private void LoadCsvFile()
		{
			//对数据的有效性进行验证
		
			if (this.fileName == null)
			{
				throw new Exception("请指定要载入的CSV文件名");
			}
			else if (!File.Exists(this.fileName))
			{
				throw new Exception("指定的CSV文件不存在，fileName:"+fileName);
			}
			else
			{
			}
			if (this.encoding == null)
			{
				this.encoding = Encoding.Default;
			}
		
			StreamReader sr = new StreamReader(this.fileName,this.encoding); 
			string   csvDataLine;
		
			csvDataLine = "";
			while (true)
			{
				string fileDataLine;
			
				fileDataLine = sr.ReadLine();
				if (fileDataLine == null)
				{
					break;
				}
				if (csvDataLine == "")
				{
					csvDataLine = fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
				}
				else
				{
					csvDataLine += "/r/n" + fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
				}
				//如果包含偶数个引号，说明该行数据中出现回车符或包含逗号
				if (!IfOddQuota(csvDataLine))
				{
					AddNewDataLine(csvDataLine);
					csvDataLine = "";
				}
			}
        
			sr.Close();
			//数据行出现奇数个引号
			if (csvDataLine.Length > 0)
			{
				throw new Exception("CSV文件的格式有错误");
			}
		}
	
		/// <summary>
		/// 获取两个连续引号变成单个引号的数据行
		/// </summary>
		/// <param name="fileDataLine">文件数据行</param>
		/// <returns></returns>
		private string GetDeleteQuotaDataLine(string fileDataLine)
		{
			return fileDataLine.Replace("\"\"","\"");
		}
	
		/// <summary>
		/// 判断字符串是否包含奇数个引号
		/// </summary>
		/// <param name="dataLine">数据行</param>
		/// <returns>为奇数时，返回为真；否则返回为假</returns>
		private bool IfOddQuota(string dataLine)
		{
			int  quotaCount;
			bool oddQuota;
		
			quotaCount = 0;
			for (int i = 0;i < dataLine.Length;i++)
			{
				if (dataLine[i] == '\"')
				{
					quotaCount++;
				}
			}
		
			oddQuota = false;
			if (quotaCount % 2 == 1)
			{
				oddQuota = true;
			}   
		
			return oddQuota;
		}
	
		/// <summary>
		/// 判断是否以奇数个引号开始
	
		/// </summary>
		/// <param name="dataCell"></param>
		/// <returns></returns>
		private bool IfOddStartQuota(string dataCell)
		{
			int  quotaCount;
			bool oddQuota;
		
			quotaCount = 0;
			for (int i = 0;i < dataCell.Length;i++)
			{
				if (dataCell[i] == '\"')
				{
					quotaCount++;
				}
				else
				{
					break;
				}
			}
		
			oddQuota = false;
			if (quotaCount % 2 == 1)
			{
				oddQuota = true;
			}   
		
			return oddQuota;
		}
	
		/// <summary>
		/// 判断是否以奇数个引号结尾
		/// </summary>
		/// <param name="dataCell"></param>
		/// <returns></returns>
		private bool IfOddEndQuota(string dataCell)
		{
			int  quotaCount;
			bool oddQuota;
		
			quotaCount = 0;
			for (int i = dataCell.Length -1;i >= 0;i--)
			{
				if (dataCell[i] == '\"')
				{
					quotaCount++;
				}
				else
				{
					break;
				}
			}
		
			oddQuota = false;
			if (quotaCount % 2 == 1)
			{
				oddQuota = true;
			}   
		
			return oddQuota;
		}
	
		/// <summary>
		/// 加入新的数据行
	
		/// </summary>
		/// <param name="newDataLine">新的数据行</param>
		private void AddNewDataLine(string newDataLine)
		{
			Debug.Log("NewLine:" + newDataLine);
		
			//return;
		
			ArrayList colAL = new ArrayList();
			string[] dataArray = newDataLine.Split(',');
			bool  oddStartQuota;       //是否以奇数个引号开始
		
			string      cellData;
		
			oddStartQuota = false;
			cellData = "";
			for (int i = 0 ;i < dataArray.Length;i++)
			{
				if (oddStartQuota)
				{
					//因为前面用逗号分割,所以要加上逗号
					cellData += "," + dataArray[i];
					//是否以奇数个引号结尾
					if (IfOddEndQuota(dataArray[i]))
					{
						colAL.Add(GetHandleData(cellData));
						oddStartQuota = false;
						continue;
					}
				}
				else
				{
					//是否以奇数个引号开始
				
					if (IfOddStartQuota(dataArray[i]))
					{
						//是否以奇数个引号结尾,不能是一个双引号,并且不是奇数个引号
					
						if (IfOddEndQuota(dataArray[i]) && dataArray[i].Length > 2 && !IfOddQuota(dataArray[i]))
						{
							colAL.Add(GetHandleData(dataArray[i]));
							oddStartQuota = false;
							continue;
						}
						else
						{
						
							oddStartQuota = true;  
							cellData = dataArray[i];
							continue;
						}
					} 
					else
					{
						colAL.Add(GetHandleData(dataArray[i])); 
					}
				}           
			}

			if (oddStartQuota)
			{
				throw new Exception("数据格式有问题");
			}
			this.rowAL.Add(colAL);
		}
	
	
		/// <summary>
		/// 去掉格子的首尾引号，把双引号变成单引号
	
		/// </summary>
		/// <param name="fileCellData"></param>
		/// <returns></returns>
		private string GetHandleData(string fileCellData)
		{
			if (fileCellData == "")
			{
				return "";
			}
			if (IfOddStartQuota(fileCellData))
			{
				if (IfOddEndQuota(fileCellData))
				{
					return fileCellData.Substring(1,fileCellData.Length-2).Replace("\"\"","\""); //去掉首尾引号，然后把双引号变成单引号
				}
				else
				{
					throw new Exception("数据引号无法匹配" + fileCellData);
				}    
			}
			else
			{
				//考虑形如""    """"      """"""   
				if (fileCellData.Length >2 && fileCellData[0] == '\"')
				{
					fileCellData = fileCellData.Substring(1,fileCellData.Length-2).Replace("\"\"","\""); //去掉首尾引号，然后把双引号变成单引号
				}
			}
		
			return fileCellData;
		}
	}
}
