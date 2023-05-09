using ECG.Contracts;
using ECG.Core.MySql;
using ECG.Core.Oracle;
using ECG.Core.SqlServer;

namespace ECG.Core; 

public sealed class DatabaseApiFactory {
    public static IDatabaseApi? CreateDatabaseApi(string connectionString,
        DatabaseType databaseType = DatabaseType.SqlServer) {
        IDatabaseApi? databaseProvider = null;
        switch (databaseType) {
            case DatabaseType.SqlServer:
                databaseProvider = new SqlServerDatabaseApi(connectionString);
                break;
            case DatabaseType.MySql:
                databaseProvider = new MySqlDatabaseApi(connectionString);
                break;
            case DatabaseType.Oracle:
                databaseProvider = new OracleDatabaseApi(connectionString);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
        }
        return databaseProvider;
    }
}