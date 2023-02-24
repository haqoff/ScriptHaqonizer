using FluentAssertions;
using Microsoft.Data.SqlClient;
using ScriptHaqonizer.Console.Tests.Base;
using ScriptHaqonizer.Console.Tests.MsSql.Helpers;
using ScriptHaqonizer.MsSql.Database.Constants;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.MsSql.Sets.Set1_CreateTableInExistingDatabase;

public class MsSqlSet1Tests : MsSqlTestBase
{
    public MsSqlSet1Tests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void ShouldMigrateSuccessfully_When2DatabasesAndFullMode()
    {
        CreateDatabase("Db1");
        CreateDatabase("Db2");

        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d Db2 -s {ScriptDirPath.Escape()} -e Development -m Full -t MsSql --from Test");

        Assert.Equal(ExitConstants.MigrationSuccess, result.ExitCode);
        GetExecutedScripts("Db2").Should().Contain(s => s.Id == 1 && s.ScriptName == "ScriptName");
    }

    [Fact]
    public async Task ShouldMigrateSuccessfullyAndBackup_WhenFullModeAndBackupSpecified()
    {
        CreateDatabase("Db1");

        const string backupPath = "/usr/backups";
        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d Db1 -s {ScriptDirPath.Escape()} -e Development -m Full -t MsSql --from Test --backupPath {backupPath}");

        Assert.Equal(ExitConstants.MigrationSuccess, result.ExitCode);
        GetExecutedScripts("Db1").Should().Contain(s => s.Id == 1 && s.ScriptName == "ScriptName");
        var allBackupsPathLsCommand = await Container.ExecAsync(new List<string> {"sh", "-c", $@"cd {backupPath} && ls -a | sort"});
        var lastBackupFolder = allBackupsPathLsCommand.Stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries)[^1];
        var lastBackupFolderPath = backupPath + "/" + lastBackupFolder;
        var lastBackupPathLsCommand = await Container.ExecAsync(new List<string> {"sh", "-c", $@"cd ""{lastBackupFolderPath}"" && ls -a | sort"});
        lastBackupPathLsCommand.Stdout.Should().Contain("Db1.bak");
    }

    [Fact]
    public void ShouldCheckSuccessfully_When2DatabasesAndOnlyCheckMode()
    {
        CreateDatabase("Db1");
        CreateDatabase("Db2");

        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d Db2 -s {ScriptDirPath.Escape()} -e Development -m OnlyCheck -t MsSql --from Test");

        Assert.Equal(ExitConstants.MigrationSuccess, result.ExitCode);
        using var connection = CreateOpenedConnection();
        var cmd = new SqlCommand($"SELECT OBJECT_ID('{MsSqlTableConstants.MigrationScriptTableName}', 'U')", connection);
        var value = cmd.ExecuteScalar();
        Assert.Equal(DBNull.Value, value);
    }
}