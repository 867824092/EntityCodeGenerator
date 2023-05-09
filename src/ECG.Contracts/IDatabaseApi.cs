
using System.Data.Common;

namespace ECG.Contracts; 
/// <summary>
/// 数据库提供者
/// </summary>
public interface IDatabaseApi {
    /// <summary>
    /// 获取数据库下所有表名
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<string>> QueryAllTablesNameAsync();
    /// <summary>
    /// 获取表结构描述
    /// </summary>
    Task<TableStructure> QueryTableDescriptionAsync(string tableName,string @namespace,CancellationToken cancellationToken = default);
    
}