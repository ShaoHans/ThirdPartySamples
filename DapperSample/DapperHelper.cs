using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Dapper.SqlMapper;

namespace DapperSample
{
    public class DapperHelper : IDisposable
    {
        public IDbConnection DbConnection { get; }

        private string _connectionString = @"Server=.;DataBase=T3FX.PM;uid=sa;pwd=sa;";

        public DapperHelper(string connectionString = "")
        {
            if(!string.IsNullOrWhiteSpace(connectionString))
            {
                _connectionString = connectionString;
            }

            DbConnection = new SqlConnection(_connectionString);
        }

        public IDbTransaction BeginTransaction() => DbConnection.BeginTransaction();
        public IDbTransaction BeginTransaction(IsolationLevel il) => DbConnection.BeginTransaction(il);
        public void ChangeDatabase(string databaseName) => DbConnection.ChangeDatabase(databaseName);
        public void Close() => DbConnection.Close();
        public IDbCommand CreateCommand() => DbConnection.CreateCommand();

        public int Execute(string sql, object param = null) => DbConnection.Execute(sql, param);
        public IDataReader ExecuteReader(string sql, object param = null) => DbConnection.ExecuteReader(sql, param);
        public object ExecuteScalar(string sql, object param = null) => DbConnection.ExecuteScalar(sql, param);
        public GridReader QueryMultiple(string sql, object param = null) => DbConnection.QueryMultiple(sql, param);

        public IEnumerable<dynamic> Query(string sql, object param = null) => DbConnection.Query(sql, param);
        public dynamic QueryFirst(string sql, object param = null) => DbConnection.QueryFirst(sql, param);
        public dynamic QueryFirstOrDefault(string sql, object param = null) => DbConnection.QueryFirstOrDefault(sql, param);
        public dynamic QuerySingle(string sql, object param = null) => DbConnection.QuerySingle(sql, param);
        public dynamic QuerySingleOrDefaul(string sql, object param = null) => DbConnection.QuerySingleOrDefault(sql, param);

        public IEnumerable<T> Query<T>(string sql, object param = null) => DbConnection.Query<T>(sql, param);
        public IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null) =>
            DbConnection.Query<TFirst, TSecond, TReturn>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);

        public T QueryFirst<T>(string sql, object param = null) => DbConnection.QueryFirst<T>(sql, param);
        public T QueryFirstOrDefault<T>(string sql, object param = null) => DbConnection.QueryFirstOrDefault<T>(sql, param);
        public T QuerySingle<T>(string sql, object param = null) => DbConnection.QuerySingle<T>(sql, param);
        public T QuerySingleOrDefault<T>(string sql, object param = null) => DbConnection.QuerySingleOrDefault<T>(sql, param);

        public Task<int> ExecuteAsync(string sql, object param = null) => DbConnection.ExecuteAsync(sql, param);
        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null) => DbConnection.ExecuteReaderAsync(sql, param);
        public Task<object> ExecuteScalarAsync(string sql, object param = null) => DbConnection.ExecuteScalarAsync(sql, param);
        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null) => DbConnection.QueryMultipleAsync(sql, param);

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null) => DbConnection.QueryAsync(sql, param);
        public Task<dynamic> QueryFirstAsync(string sql, object param = null) => DbConnection.QueryFirstAsync(sql, param);
        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null) => DbConnection.QueryFirstOrDefaultAsync(sql, param);
        public Task<dynamic> QuerySingleAsync(string sql, object param = null) => DbConnection.QuerySingleAsync(sql, param);
        public Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param = null) => DbConnection.QuerySingleOrDefaultAsync(sql, param);

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null) => DbConnection.QueryAsync<T>(sql, param);
        public Task<T> QueryFirstAsync<T>(string sql, object param = null) => DbConnection.QueryFirstAsync<T>(sql, param);
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null) => DbConnection.QueryFirstOrDefaultAsync<T>(sql, param);
        public Task<T> QuerySingleAsync<T>(string sql, object param = null) => DbConnection.QuerySingleAsync<T>(sql, param);
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null) => DbConnection.QuerySingleOrDefaultAsync<T>(sql, param);

        public void Dispose()
        {
            if (DbConnection != null)
            {
                DbConnection.Close();
                DbConnection.Dispose();
            }
        }
    }
}
