using System.Data.Common;
using System.Runtime.CompilerServices;
using Dapper;
using ECG.Contracts;
using Oracle.ManagedDataAccess.Client;

namespace ECG.Core.Oracle; 

internal class OracleDatabaseApi : AbstractDatabaseApi {
    public OracleDatabaseApi(string connectionString) : base(connectionString) { }
    protected override string SELECT_ALL_TABLES_NAME => "SELECT table_name FROM user_tables";
    protected override string SELECT_TABLE_COLUMN_COUNT => "SELECT COUNT(*) FROM user_tab_columns WHERE table_name = '{0}'";

    protected override string SELECT_TABLE_COLUMN_DESCRIPTION =>
        "SELECT column_name,data_type,data_length,data_precision,data_scale,nullable,data_default FROM user_tab_columns WHERE table_name = '{0}'";

    protected override DbConnection GetDbConnection() {
        return new OracleConnection(_connectionString);
    }

    protected override Action<DbDataReader, ColumnDescription> ColumnMap() {
        return (reader, obj) => {
            obj.ColumnName = reader.GetString(0);
            int? precision = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
            int? scale = reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4);
            obj.ColumnType = ClrTypeMap(reader.GetString(1),precision, scale);
            obj.IsCanNull = reader.GetString(5) == "Y";
        };
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private string ClrTypeMap(string columnType,int? precision,int? scale) {
        switch (columnType.ToLower())
        {
            case "number" when precision == 19 && scale == 0:
                return "long";
            case "number" when precision == 10 && scale == 0:
                return "int";
            case "number" when precision == 5 && scale == 0:
                return "Int16";
            case "number" when precision == 4 && scale == 0:
                return "byte";
            case "number" when precision == 1 && scale == 0:
                return "bool";
            case "number":
                return "decimal";
            case "varchar2":
            case "nvarchar2":
            case "char":
            case "nchar":
            case "clob":
                return "string";
            case "date":
            case "timestamp":
            case "timestamp with time zone":
            case "timestamp with local time zone":
                return "DateTime";
            case "float":
            case "binary_float":
                return "float";
            case "double precision":
            case "binary_double":
                return "double";
            case "blob":
            case "raw":
                return "byte[]";
            default:
                throw new InvalidOperationException($"Invalid Oracle type '{columnType}'.");
        }
    }
}