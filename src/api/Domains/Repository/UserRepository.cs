using API.Configurations.Factories;
using API.Domains.Models;
using API.Domains.Services;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Domains.Repository
{
    public class UserRepository : SqlService
    {
        public UserRepository(IDatabaseFactory databaseFactory, ILogger<SqlService> logger) : base(databaseFactory, logger) { }
            public override async Task<IEnumerable<User>> ListUsersAsync(string sql, object parameters)
            {
                _logger.LogDebug($"QUERY LIST COMMAND | { sql }");
                _logger.LogDebug($"QUERY LIST PARAMETERS | { parameters }");

                var transaction = _databaseFactory.BeginTransactionAsync();

                var command = new CommandDefinition(sql, parameters, transaction);

                var usuarios = await _databaseFactory.Connection().QueryAsync<User,Contact,User>(command,
                    (user, contact) => {
                        //user.Contacts.Add(contact);
                        return user; 
                    },splitOn: "ContactId");

                usuarios = usuarios.GroupBy(u => u.UserId).Select(user =>
                {
                    var groupedUser =  user.First();
                    groupedUser.Contacts = user.Select(c => c.Contacts.Single()).ToList();
                    return groupedUser;
                });

                _logger.LogDebug($"QUERY LIST EXECUTED");

                return usuarios;
            }
    }
}
