using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Hosting.Tests;

public class ResolvingTests
{
    [Fact]
    public void ShouldResolveHostedService_WhenMsSqlDbType()
    {
        var logger = new Mock<ILogger>();
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddScoped(_ => logger.Object);
        serviceCollection.AddScriptHaqonizer(new MigrationOptions()
        {
            DatabaseName = "dbName",
            BackupPath = "c:/backups",
            ConnectionString = "ConnectionString",
            DbType = SupportedDb.MsSql,
            EnvironmentName = AvailableEnvironment.Development,
            Mode = ExecutionMode.Full,
            ScriptDirectoryPath = "c:/scripts",
            Source = "Tests",
            SpecifyingDatabaseNameValidation = true
        });
        var provider = serviceCollection.BuildServiceProvider();
        var service = provider.GetRequiredService<IHostedService>();
        Assert.IsType<MigrationHostedService>(service);
    }
}