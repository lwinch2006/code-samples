using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;
using Dka.Net5.IdentityWithDapper.Infrastructure.Utils;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories.MSSQL
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;
        
        public UserLoginRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }
        
        public async Task<UserLoginDto> Create(CreateUserLoginDto createUserLoginDto)
        {
            const string query = @"
                    INSERT INTO [UserLogins] ([LoginProvider], [ProviderKey], [ProviderDisplayName], [UserId])
                    VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId);
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
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

            using (var connection = _dbConnectionFactory.GetDbConnection())
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

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var userLogins = await connection.QueryAsync<UserLogin>(query, new { @UserId = userId });
                var userLoginsDto = _mapper.Map<IEnumerable<UserLoginDto>>(userLogins);
                return userLoginsDto;
            }
        }
    }
}