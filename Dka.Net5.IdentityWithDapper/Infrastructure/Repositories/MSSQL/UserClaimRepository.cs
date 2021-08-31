using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;
using Dka.Net5.IdentityWithDapper.Infrastructure.Utils;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories.MSSQL
{
    public class UserClaimRepository : IUserClaimRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;
        
        public UserClaimRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }        
        
        public async Task<IEnumerable<UserClaimDto>> Get(Guid userId)
        {
            const string query = @"
                SELECT *
                FROM [UserClaims]
                WHERE [UserId] = @UserId;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var userClaims = await connection.QueryAsync<UserClaim>(query, new { @UserId = userId });
                var userClaimsDto = _mapper.Map<IEnumerable<UserClaimDto>>(userClaims);
                return userClaimsDto;
            }
        }

        public async Task<int> Create(IEnumerable<CreateUserClaimDto> createUserClaimsDto)
        {
            const string query = @"
                INSERT INTO [UserClaims] ([ClaimType], [ClaimValue], [UserId])
                VALUES (@ClaimType, @ClaimValue, @UserId);
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var insertedRowsNum = await connection.ExecuteAsync(query, createUserClaimsDto);
                return insertedRowsNum;
            }
        }

        public async Task<int> Update(UpdateUserClaimDto updateUserClaimDto)
        {
            const string query = @"
                UPDATE [UserClaims]
                SET 
                    [ClaimType] = @NewClaimType,
                    [ClaimValue] = @NewClaimValue
                WHERE
                    [UserId] = @UserId
                AND [ClaimType] = @OldClaimType
                AND [ClaimValue] = @OldClaimValue
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var updatedRowsNum = await connection.ExecuteAsync(query, updateUserClaimDto);
                return updatedRowsNum;
            }
        }

        public async Task<int> Delete(IEnumerable<DeleteUserClaimDto> deleteUserClaimsDto)
        {
            const string query = @"
                DELETE FROM [UserClaims]
                WHERE [UserId] = @UserId
                AND [ClaimType] = @ClaimType
                AND [ClaimValue] = @ClaimValue;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var deletedRowsNum = await connection.ExecuteAsync(query, deleteUserClaimsDto);
                return deletedRowsNum;
            }
        }
    }
}