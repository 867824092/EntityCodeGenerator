using ECG.Contracts;

namespace ECG.Core.Tests; 

public abstract class DatabaseApiTestBase {
    
    protected DatabaseApiTestBase(DatabaseType databaseType,
        string connectionString) {
        DatabaseApi = DatabaseApiFactory.CreateDatabaseApi(connectionString,databaseType);
    }
    protected IDatabaseApi? DatabaseApi { get; }
}