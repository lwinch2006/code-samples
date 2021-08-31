using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.RoleClaim;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;
using Dka.Net5.IdentityWithDapper.Infrastructure.Utils;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories.SQLite
{
    public class RoleClaimRepository : IRoleClaimRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;

        public RoleClaimRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleClaimDto>> Get(Guid roleId)
        {
            const string query = @"
                SELECT *
                FROM [RoleClaims]
                WHERE [RoleId] = @RoleId;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var roleClaims = await connection.QueryAsync<RoleClaim>(query, new { @RoleId = roleId });
                var roleClaimsDto = _mapper.Map<IEnumerable<RoleClaimDto>>(roleClaims);
                return roleClaimsDto;
            }
        }

        public async Task<RoleClaimDto> Create(CreateRoleClaimDto createRoleClaimDto)
        {
            const string query = @"
                INSERT INTO [RoleClaims] ([ClaimType], [ClaimValue], [RoleId])
                VALUES (@ClaimType, @ClaimValue, @RoleId);

                SELECT last_insert_rowid();
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var insertedId = await connection.QuerySingleAsync<int>(query, createRoleClaimDto);

                if (insertedId == 0)
                {
                    return null;
                }

                var roleClaimDto = _mapper.Map<RoleClaimDto>(createRoleClaimDto);
                roleClaimDto.Id = insertedId;

                return roleClaimDto;                
            }
        }

        public async Task<int> Delete(DeleteRoleClaimDto deleteRoleClaimDto)
        {
            const string query = @"
                DELETE FROM [RoleClaims]
                WHERE [RoleId] = @RoleId
                AND [ClaimType] = @ClaimType
                AND [ClaimValue] = @ClaimValue;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var deletedRowsNum = await connection.ExecuteAsync(query, deleteRoleClaimDto);

                return deletedRowsNum;
            }
        }
    }
}