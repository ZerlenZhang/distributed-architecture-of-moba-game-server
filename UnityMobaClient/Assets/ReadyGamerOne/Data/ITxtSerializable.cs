using System.IO;

namespace ReadyGamerOne.Data
{
	/// <summary>
	/// 想要通过TxtManager
	/// </summary>
	public interface ITxtSerializable
	{
		/// <summary>
		/// 标志位
		/// </summary>
		string Sign { get; }
		
		/// <summary>
		/// 读取标志行
		/// </summary>
		/// <param name="initLine"></param>
		void SignInit(string initLine);
		
		/// <summary>
		/// 读取大括号以内的部分
		/// </summary>
		/// <param name="sr"></param>
		void LoadTxt(StreamReader sr);
	}
}

