using System.Data.Common;
using System.Runtime.CompilerServices;
using Dapper;
using ECG.Contracts;
using MySqlConnector;

namespace ECG.Core.MySql; 

internal class MySqlDatabaseApi : AbstractDatabaseApi {
    public MySqlDatabaseApi(string connectionString) : base(connectionString) { }
    protected override string SELECT_ALL_TABLES_NAME => "SHOW TABLES;";
    protected override string SELECT_TABLE_COLUMN_COUNT => "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE table_name = '{0}';";
    protected override string SELECT_TABLE_COLUMN_DESCRIPTION => "SELECT COLUMN_NAME,DATA_TYPE,IS_NULLABLE,COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}';";

    protected override DbConnection GetDbConnection() {
        return new MySqlConnection(_connectionString);
    }

    protected override Action<DbDataReader, ColumnDescription> ColumnMap() {
        return (reader, obj) => {
            obj.ColumnName = reader.GetString(0);
            obj.ColumnType = ClrTypeMap(reader.GetString(1));
            obj.IsCanNull = reader.GetString(2) == "YES";
            obj.Comment = reader.GetString(3);
        };
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private string ClrTypeMap(string columnType) {
        switch (columnType.ToLower())
        {
            case "tinyint":
            case "smallint":
            case "mediumint":
            case "int":
            case "integer":
                return "int";
            case "bigint":
                return "long";
            case "float":
                return "float";
            case "double":
            case "real":
                return "double";
            case "decimal":
            case "numeric":
                return "decimal";
            case "datetime":
            case "date":
            case "timestamp":
                return "DateTime";
            case "time":
                return "TimeSpan";
            case "year":
                return "short";
            case "bit":
            case "bool":
            case "boolean":
                return "bool";
            case "char":
            case "varchar":
            case "text":
            case "tinytext":
            case "mediumtext":
            case "longtext":
            case "enum":
            case "set":
                return "string";
            case "binary":
            case "varbinary":
            case "blob":
            case "tinyblob":
            case "mediumblob":
            case "longblob":
                return "byte[]";
            case "geometry":
            case "point":
            case "linestring":
            case "polygon":
            case "multipoint":
            case "multilinestring":
            case "multipolygon":
            case "geometrycollection":
                return "object";
            default:
                throw new ArgumentException($"Unknown MySQL data type: {columnType}");
        }
    }
}