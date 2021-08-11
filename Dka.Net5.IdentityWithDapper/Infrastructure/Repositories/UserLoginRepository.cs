using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;
using Dka.Net5.IdentityWithDapper.Utils.Constants;
using Microsoft.Extensions.Configuration;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserLoginRepository
    {
        Task<UserLoginDto> Create(CreateUserLoginDto createUserLoginDto);
        Task<int> Delete(DeleteUserLoginDto deleteUserLoginDto);
        Task<IEnumerable<UserLoginDto>> Get(Guid userId);
    }
    
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        
        public UserLoginRepository(IConfiguration configuration, IMapper mapper)
        {
            _connectionString = configuration[SystemConstants.DbConnectionConfigParamName];
            _mapper = mapper;
        }
        
        public async Task<UserLoginDto> Create(CreateUserLoginDto createUserLoginDto)
        {
            const string query = @"
                    INSERT INTO [UserLogins] ([LoginProvider], [ProviderKey], [ProviderDisplayName], [UserId])
                    VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId);
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var insertedRowsNum = await connection.ExecuteAsync(query, createUserLoginDto);

                if (insertedRowsNum == 0)
                {
                    return null;
                }

                var userLoginDto = _mapper.Map<UserLoginDto>(createUserLoginDto);

                return userLoginDto;                  
            }
        }

        public async  Task<int> Delete(DeleteUserLoginDto deleteUserLoginDto)
        {
            const string query = @"
                DELETE FROM [UserLogins]
                WHERE [UserId] = @UserId
                AND [LoginProvider] = @LoginProvider
                AND [ProviderKey] = @ProviderKey;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var deletedRowsNum = await connection.ExecuteAsync(query, deleteUserLoginDto);
                return deletedRowsNum;
            }
        }

        public async Task<IEnumerable<UserLoginDto>> Get(Guid userId)
        {
            const string query = @"
                SELECT *
                FROM [UserLogins]
                WHERE [UserId] = @UserId;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var userLogins = await connection.QueryAsync<UserLogin>(query, new { @UserId = userId });
                var userLoginsDto = _mapper.Map<IEnumerable<UserLoginDto>>(userLogins);
                return userLoginsDto;
            }
        }
    }
}