using System.Data;

namespace ScriptHaqonizer.Console.Tests.Base;

public static class ConnectionExtensions
{
    public static int ExecuteNonQuery(this IDbConnection connection, string commandText)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = commandText;
        return cmd.ExecuteNonQuery();
    }
}