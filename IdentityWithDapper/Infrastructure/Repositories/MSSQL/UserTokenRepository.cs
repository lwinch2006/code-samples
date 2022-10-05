using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using IdentityWithDapper.Infrastructure.Models.DTO.UserToken;
using IdentityWithDapper.Infrastructure.Utils;

namespace IdentityWithDapper.Infrastructure.Repositories.MSSQL
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;
        
        public UserTokenRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
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

            using (var connection = _dbConnectionFactory.GetDbConnection())
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

            using (var connection = _dbConnectionFactory.GetDbConnection())
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