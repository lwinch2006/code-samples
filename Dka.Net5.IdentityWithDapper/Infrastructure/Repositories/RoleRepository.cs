using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.Role;
using Dka.Net5.IdentityWithDapper.Utils.Constants;
using Microsoft.Extensions.Configuration;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IRoleRepository
    {
        Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto);
        Task<int> UpdateAsync(UpdateRoleDto updateRoleDto);
        Task<int> DeleteAsync(DeleteRoleDto deleteRoleDto);
        Task<RoleDto> FindByIdAsync(Guid roleId);
        Task<RoleDto> FindByNameAsync(string normalizedRoleName);
    }
    
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        
        public RoleRepository(
            IConfiguration configuration,
            IMapper mapper)
        {
            _connectionString = configuration[SystemConstants.DbConnectionConfigParamName];
            _mapper = mapper;
        }
        
        public async Task<RoleDto> CreateAsync(CreateRoleDto createRoleDto)
        {
            const string query = @"
                INSERT INTO [Roles] (
                    [Id]
                    ,[Name]
                    ,[NormalizedName]
                    ,[ConcurrencyStamp])
                VALUES (
                    @Id
                    ,@Name
                    ,@NormalizedName
                    ,@ConcurrencyStamp);
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var insertedRowsNum = await connection.ExecuteAsync(query, createRoleDto);

                if (insertedRowsNum == 0)
                {
                    return null;
                }

                var roleDto = _mapper.Map<RoleDto>(createRoleDto);

                return roleDto;
            }
        }

        public async Task<int> UpdateAsync(UpdateRoleDto updateRoleDto)
        {
            const string query = @"
                UPDATE [Roles]
                SET [Name] = @Name
                    ,[NormalizedName] = @NormalizedName
                    ,[ConcurrencyStamp] = @ConcurrencyStamp
                WHERE [Id] = @Id;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var updatedRowsNum = await connection.ExecuteAsync(query, updateRoleDto);
                return updatedRowsNum;
            }
        }

        public async Task<int> DeleteAsync(DeleteRoleDto deleteRoleDto)
        {
            const string query = @"
                DELETE
                FROM [Roles]
                WHERE [Id] = @Id
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var deletedRowsNum = await connection.ExecuteAsync(query, new {@Id = deleteRoleDto.Id});
                return deletedRowsNum;
            }
        }

        public async Task<RoleDto> FindByIdAsync(Guid roleId)
        {
            const string query = @"
                SELECT *
                FROM [Roles]
                WHERE [Id] = @Id;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var role = await connection.QuerySingleOrDefaultAsync(query, new {@Id = roleId});
                var roleDto = _mapper.Map<RoleDto>(role);
                return roleDto;
            }
        }

        public async Task<RoleDto> FindByNameAsync(string normalizedRoleName)
        {
            const string query = @"
                SELECT *
                FROM [Roles]
                WHERE [NormalizedRoleName] = @NormalizedRoleName;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                var role = await connection.QuerySingleOrDefaultAsync(query, new {@NormalizedRoleName = normalizedRoleName});
                var roleDto = _mapper.Map<RoleDto>(role);
                return roleDto;
            }
        }
    }
}