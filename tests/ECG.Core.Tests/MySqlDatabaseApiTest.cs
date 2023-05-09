using ECG.Contracts;

namespace ECG.Core.Tests; 

public class MySqlDatabaseApiTest : DatabaseApiTestBase {
    public MySqlDatabaseApiTest() :
        base(DatabaseType.MySql, "Server=localhost;Database=BookStore;Uid=root;Pwd=123456;") {
        
    }
    [Fact]
    public async Task QueryAllTablesNameAsyncTest() {
        var result = await DatabaseApi!.QueryAllTablesNameAsync();
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("__EFMigrationsHistory")]
    public async Task QueryTableDescriptionAsyncTest(string tableName) {
        var result = await DatabaseApi!.QueryTableDescriptionAsync(tableName, "ECG.Core.Tests");
        Assert.NotNull(result);
    }
}