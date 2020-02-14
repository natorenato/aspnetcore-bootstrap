using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using API.Domains.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace API.Configurations.Factories
{
    public interface IDatabaseFactory
    {
        IDbTransaction BeginTransactionAsync();
        IDbConnection Connection();
        Task OpenConnectionAsync();
        void CommitTransaction();
        void RollbackTransaction();
        void CloseConnection();
    }

    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;
        private bool _isTransactionOpen;

        public DatabaseFactory(IOptions<Database> database)
        {
            var connectionstring = $"Password={database.Value.Password};Persist Security Info=True;User ID={database.Value.User};Initial Catalog={database.Value.Schema};Data Source={database.Value.Server},{database.Value.Port};";
            _connection = new SqlConnection(connectionstring);
        }

        public IDbTransaction BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                if (_connection.State != ConnectionState.Open)
                    throw new Exception("A conexão com o banco não esta aberta.");

                _transaction = _connection.BeginTransaction();
            }

            _isTransactionOpen = true;

            return _transaction;
        }

        public IDbConnection Connection()
        {
            return _connection;
        }

        public async Task OpenConnectionAsync()
        {
            var connection = _connection as SqlConnection;

            await connection.OpenAsync();
        }

        public void CommitTransaction()
        {
            if (_isTransactionOpen)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }

            _isTransactionOpen = false;
        }

        public void RollbackTransaction()
        {
            if (_isTransactionOpen)
                _transaction.Rollback();
        }

        public void CloseConnection()
        {
            _connection.Close();

            _connection.Dispose();
        }
    }

}