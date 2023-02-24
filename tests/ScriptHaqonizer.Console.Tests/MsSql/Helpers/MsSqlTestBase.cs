using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using ScriptHaqonizer.Console.Tests.Base;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.MsSql.Database.Constants;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.MsSql.Helpers;

public abstract class MsSqlTestBase : IAsyncLifetime
{
    protected MsSqlTestBase(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
        var currentType = GetType();
        ScriptDirPath = GetScriptDirPath(currentType);
    }


    public MsSqlTestcontainer Container { get; private set; } = default!;
    public string ConnectionString { get; private set; } = default!;
    public ITestOutputHelper OutputHelper { get; }
    public string ScriptDirPath { get; }

    public async Task InitializeAsync()
    {
        var container = await CreateContainerAsync();
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(container.ConnectionString)
        {
            TrustServerCertificate = true,
            InitialCatalog = ""
        };

        ConnectionString = sqlConnectionStringBuilder.ConnectionString;
        Container = container;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Container.DisposeAsync();
    }

    public void CreateDatabase(string name)
    {
        using var connection = CreateOpenedConnection();
        connection.ExecuteNonQuery($"CREATE DATABASE {name};");
    }

    public SqlConnection CreateOpenedConnection()
    {
        var conn = new SqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    public IReadOnlyList<ExecutedScript> GetExecutedScripts(string dbName)
    {
        using var connection = CreateOpenedConnection();
        var cmd = new SqlCommand($"SELECT * FROM {dbName}.dbo.{MsSqlTableConstants.MigrationScriptTableName}", connection);
        using var reader = cmd.ExecuteReader();
        var result = new List<ExecutedScript>();

        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var date = reader.GetDateTime(2);
            result.Add(new ExecutedScript(id, name, date));
        }

        return result;
    }

    public ConsoleRunResult ExecuteConsole(string args)
    {
        var result = ConsoleExecutor.Execute(args);
        result.Log(OutputHelper);
        return result;
    }

    private static async Task<MsSqlTestcontainer> CreateContainerAsync()
    {
        var container = new ContainerBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration
            {
                Password = "localdevpassword#123",
            })
            .WithImage("mcr.microsoft.com/mssql/server:2017-latest-ubuntu")
            .WithCleanUp(true)
            .Build();

        await container.StartAsync();
        return container;
    }

    private static string GetScriptDirPath(Type testType)
    {
        var lastDir = testType.Namespace!.Split('.')[^1];
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDir, "MsSql", "Sets", lastDir, "Scripts");
    }
}