using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserToken;
using Dka.Net5.IdentityWithDapper.Utils.Constants;
using Microsoft.Extensions.Configuration;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserTokenRepository
    {
        Task<UserTokenDto> CreateOrUpdate(CreateOrUpdateUserTokenDto createOrUpdateUserTokenDto);
        Task<UserTokenDto> Get(GetUserTokenDto getUserTokenDto);
    }
    
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        
        public UserTokenRepository(IConfiguration configuration, IMapper mapper)
        {
            _connectionString = configuration[SystemConstants.DbConnectionConfigParamName];
            _mapper = mapper;
        }
        
        public async Task<UserTokenDto> CreateOrUpdate(CreateOrUpdateUserTokenDto createOrUpdateUserTokenDto)
        {
            const string query = @"
                IF NOT EXISTS (SELECT [UserId] FROM [UserTokens] WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [Name] = @Name)
                BEGIN
                    INSERT INTO [UserTokens] ([UserId], [LoginProvider], [Name], [Value])
                    VALUES (@UserId, @LoginProvider, @Name, @Value);
                END
                ELSE
                BEGIN
                    UPDATE [UserTokens]
                    SET [Value] = @Value
                    WHERE [UserId] = @UserId
                    AND [LoginProvider] = @LoginProvider
                    AND [Name] = @Name;
                END;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var insertedRowsNum = await connection.ExecuteAsync(query, createOrUpdateUserTokenDto);

                if (insertedRowsNum == 0)
                {
                    return null;
                }

                var userTokenDto = _mapper.Map<UserTokenDto>(createOrUpdateUserTokenDto);

                return userTokenDto;                
            }
        }

        public async Task<UserTokenDto> Get(GetUserTokenDto getUserTokenDto)
        {
            const string query = @"
                SELECT *
                FROM [UserTokens]
                WHERE [UserId] = @UserId
                AND [LoginProvider] = @LoginProvider
                AND [Name] = @Name;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var userToken = await connection.QuerySingleOrDefaultAsync(
                    query,
                    new
                    {
                        @UserId = getUserTokenDto.UserId,
                        @LoginProvider = getUserTokenDto.LoginProvider,
                        @Name = getUserTokenDto.Name
                    });

                var userTokenDto = _mapper.Map<UserTokenDto>(userToken);

                return userTokenDto;
            }
        }
    }
}