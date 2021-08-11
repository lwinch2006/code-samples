using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;
using Dka.Net5.IdentityWithDapper.Utils.Constants;
using Microsoft.Extensions.Configuration;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserClaimRepository
    {
        Task<IEnumerable<UserClaimDto>> Get(Guid userId);
        Task<int> Create(IEnumerable<CreateUserClaimDto> createUserClaimsDto);
        Task<int> Update(UpdateUserClaimDto updateUserClaimDto);
        Task<int> Delete(IEnumerable<DeleteUserClaimDto> deleteUserClaimsDto);
    }
    
    public class UserClaimRepository : IUserClaimRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        
        public UserClaimRepository(IConfiguration configuration, IMapper mapper)
        {
            _connectionString = configuration[SystemConstants.DbConnectionConfigParamName];
            _mapper = mapper;
        }        
        
        public async Task<IEnumerable<UserClaimDto>> Get(Guid userId)
        {
            const string query = @"
                SELECT *
                FROM [UserClaims]
                WHERE [UserId] = @UserId;
            ";

            await using (var connection = new SqlConnection(_connectionString))
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

            await using (var connection = new SqlConnection(_connectionString))
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

            await using (var connection = new SqlConnection(_connectionString))
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

            await using (var connection = new SqlConnection(_connectionString))
            {
                var deletedRowsNum = await connection.ExecuteAsync(query, deleteUserClaimsDto);
                return deletedRowsNum;
            }
        }
    }
}