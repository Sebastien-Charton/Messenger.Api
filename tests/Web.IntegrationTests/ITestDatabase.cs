using System.Data.Common;

namespace Web.IntegrationTests;

public interface ITestDatabase
{
    Task InitialiseAsync();

    DbConnection GetConnection();

    Task DisposeAsync();
}
