using System;

namespace ReadyGamerOne.Data
{
    /// <summary>
    /// 可以使用Csv数据加载的都可以实现此接口
    /// </summary>
    public interface IUseCsvData
    {
        
        CsvMgr CsvData { get; }
        
        /// <summary>
        /// Csv生成的Item的类型定义类
        /// </summary>
        Type DataType { get; }
        
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="data"></param>
        void LoadData(CsvMgr data);
    }
}