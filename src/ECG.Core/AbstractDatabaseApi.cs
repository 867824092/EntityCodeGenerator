using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Dapper;
using ECG.Contracts;

namespace ECG.Core; 

internal abstract class AbstractDatabaseApi : IDatabaseApi {
    protected readonly string _connectionString;
    protected abstract string SELECT_ALL_TABLES_NAME { get; }
    protected abstract string SELECT_TABLE_COLUMN_COUNT { get; }
    protected abstract string SELECT_TABLE_COLUMN_DESCRIPTION { get; }
    protected AbstractDatabaseApi(string connectionString) {
        _connectionString = connectionString;
    }
    protected abstract DbConnection GetDbConnection();
    public async Task<IEnumerable<string>> QueryAllTablesNameAsync() {
        return await ExecuteAsync(async dbConnection => {
            var result = await dbConnection.ExecuteReaderAsync(SELECT_ALL_TABLES_NAME);
            List<string> tables = new();
            while (await result.ReadAsync()) {
                tables.Add(result.GetString(0));
            }
            return tables;
        });
    }

    public  Task<TableStructure> QueryTableDescriptionAsync(string tableName,
        string @namespace,
        CancellationToken cancellationToken = default) {
        return ExecuteAsync(async dbConnection => {
            int count = await dbConnection.QuerySingleAsync<int>(string.Format(SELECT_TABLE_COLUMN_COUNT, tableName));
            var tableStructure = new TableStructure(@namespace,tableName,count);
            var dbDataReader =
                await dbConnection.ExecuteReaderAsync(string.Format(SELECT_TABLE_COLUMN_DESCRIPTION, tableName));
            while (await dbDataReader.ReadAsync(cancellationToken)) {
                var obj = new ColumnDescription();
                ColumnMap()(dbDataReader,obj);
                tableStructure.Columns.Add(obj);
;            }
            return tableStructure;
        });
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract Action<DbDataReader,ColumnDescription> ColumnMap();
    protected async Task<T> ExecuteAsync<T>(Func<DbConnection, Task<T>> func) {
        await  using var connection = GetDbConnection();
        if (connection.State == ConnectionState.Closed) {
            await connection.OpenAsync();
        }
       
        return await func(connection);
    }
    protected async Task ExecuteAsync(Func<DbConnection, Task> func) {
        await  using var connection = GetDbConnection();
        if (connection.State == ConnectionState.Closed) {
            await connection.OpenAsync();
        }
        await func(connection);
    }
}