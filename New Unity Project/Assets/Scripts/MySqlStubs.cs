using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySql.Data.MySqlClient
{
    public class MySqlConnection : IDisposable, IAsyncDisposable
    {
        public MySqlConnection(string connectionString) { }
        public void Open() { }
        public Task OpenAsync() => Task.CompletedTask;
        public void Close() { }
        public void Dispose() { }
        public ValueTask DisposeAsync() => default;
    }

    public class MySqlParameterCollection
    {
        public void AddWithValue(string parameterName, object? value) { }
    }

    public class MySqlCommand : IDisposable, IAsyncDisposable
    {
        public MySqlCommand(string commandText, MySqlConnection connection) { Parameters = new MySqlParameterCollection(); }
        public MySqlParameterCollection Parameters { get; }
        public long LastInsertedId { get; set; }
        public int ExecuteNonQuery() => 0;
        public Task<int> ExecuteNonQueryAsync() => Task.FromResult(0);
        public object? ExecuteScalar() => null;
        public MySqlDataReader ExecuteReader() => new MySqlDataReader();
        public Task<MySqlDataReader> ExecuteReaderAsync() => Task.FromResult(new MySqlDataReader());
        public void Dispose() { }
        public ValueTask DisposeAsync() => default;
    }

    public class MySqlDataReader : IDisposable, IAsyncDisposable
    {
        public int FieldCount => 0;
        public void Dispose() { }
        public ValueTask DisposeAsync() => default;
        public bool Read() => false;
        public Task<bool> ReadAsync() => Task.FromResult(false);
        public string GetString(string name) => string.Empty;
        public int GetInt32(string name) => 0;
        public DateTime GetDateTime(string name) => DateTime.MinValue;
        public bool IsDBNull(int ordinal) => true;
        public Task<bool> IsDBNullAsync(int ordinal) => Task.FromResult(true);
        public string GetName(int i) => string.Empty;
        public object? GetValue(int i) => null;
        public void Close() { }
    }

    public class MySqlException : Exception
    {
        public MySqlException() { }
    }
}

namespace MySqlConnector
{
    using MySql.Data.MySqlClient;

    public class MySqlConnection : MySql.Data.MySqlClient.MySqlConnection
    {
        public MySqlConnection(string cs) : base(cs) { }
    }

    public class MySqlCommand : MySql.Data.MySqlClient.MySqlCommand
    {
        public MySqlCommand(string cmdText, MySqlConnection conn) : base(cmdText, conn) { }
    }

    public class MySqlDataReader : MySql.Data.MySqlClient.MySqlDataReader { }

    public class MySqlException : Exception
    {
        public MySqlException() { }
    }
}
