using Microsoft.Data.SqlClient;
using ScriptHaqonizer.Core.Exceptions;

namespace ScriptHaqonizer.MsSql.Database.Helpers;

/// <summary>
/// Provides helper methods for working with MSSQL.
/// </summary>
internal static class MsSqlDbHelper
{
    /// <summary>
    /// Opens a new connection.
    /// </summary>
    /// <exception cref="CannotConnectException">Exception that is thrown when a connection to the database fails.</exception>
    internal static SqlConnection GetOpenedConnection(string connectionString)
    {
        try
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        catch (Exception ex)
        {
            throw new CannotConnectException(ex);
        }
    }

    /// <summary>
    /// Gets an indication that the specified database exists.
    /// </summary>
    /// <param name="connection">Connection.</param>
    /// <param name="databaseName">Database name.</param>
    /// <param name="transaction">Transaction, must be specified if it was created for the specified <paramref name="connection"/>.</param>
    internal static bool IsDatabaseExist(SqlConnection connection, string databaseName, SqlTransaction? transaction = null)
    {
        using var command = new SqlCommand("SELECT db_id(@DatabaseName)", connection);
        command.Transaction = transaction;
        command.Parameters.AddWithValue("@DatabaseName", databaseName);

        var databaseExists = command.ExecuteScalar() != DBNull.Value;
        return databaseExists;
    }
}