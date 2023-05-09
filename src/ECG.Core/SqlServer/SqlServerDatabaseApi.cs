using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Dapper;
using ECG.Contracts;
using Microsoft.Data.SqlClient;

namespace ECG.Core.SqlServer; 

internal class SqlServerDatabaseApi : AbstractDatabaseApi {
    public SqlServerDatabaseApi(string connectionString) : base(connectionString) { }
    protected override string SELECT_ALL_TABLES_NAME => "SELECT name FROM sysobjects WHERE xtype='U' ORDER BY name";
    protected override string SELECT_TABLE_COLUMN_COUNT => "SELECT COUNT(*) FROM SYSCOLUMNS WHERE ID=OBJECT_ID('{0}')";
    protected override string SELECT_TABLE_COLUMN_DESCRIPTION => "SELECT COLUMN_NAME,DATA_TYPE,IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}';";

    protected override DbConnection GetDbConnection() {
        return new SqlConnection(_connectionString);
    }

    protected override Action<DbDataReader, ColumnDescription> ColumnMap() {
        return (reader, obj) => {
            obj.ColumnName = reader.GetString(0);
            obj.ColumnType = ClrTypeMap(reader.GetString(1));
            obj.IsCanNull = reader.GetString(2) == "YES";
        };
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private string ClrTypeMap(string columnType) {
  
            switch (columnType.ToLower())
            {
                case "bigint":
                    return "long";
                case "binary":
                case "image":
                case "timestamp":
                case "varbinary":
                    return "byte[]";
                case "bit":
                    return "bool";
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "text":
                case "varchar":
                case "xml":
                    return "string";
                case "datetime":
                case "smalldatetime":
                case "date":
                case "time":
                    return "DateTime";
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney":
                    return "decimal";
                case "float":
                    return "double";
                case "int":
                    return "int";
                case "real":
                    return "float";
                case "smallint":
                    return "short";
                case "tinyint":
                    return "byte";
                case "uniqueidentifier":
                    return "Guid";
                case "variant":
                case "udt":
                case "structured":
                    return "object";
                case "datetime2":
                case "datetimeoffset":
                    return "DateTimeOffset";
                default:
                    throw new InvalidOperationException($"Invalid SQL type '{columnType}'.");
            }
        
    }
}