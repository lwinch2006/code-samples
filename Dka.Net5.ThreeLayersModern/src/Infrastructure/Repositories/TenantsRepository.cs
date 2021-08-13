using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Infrastructure.DTO.Tenants;
using Infrastructure.Entities;
using Infrastructure.Utils;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public interface ITenantsRepository
    {
        Task<IEnumerable<Tenant>> Get();

        Task<Tenant> Get(Guid id);

        Task<Tenant> Create(CreateTenantDto createTenantDto);

        Task<int> Update(UpdateTenantDto updateTenantDto);

        Task<int> Delete(DeleteTenantDto deleteTenantDto);
    }    
    
    public class TenantsRepository : ITenantsRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;

        public TenantsRepository(IDbConnectionFactory dbConnectionFactory, IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Tenant>> Get()
        {
            const string query = @"
                SELECT *
                FROM [Tenants]
            ";

            await using (var connecion = _dbConnectionFactory.GetDbConnection())
            {
                var result = await connecion.QueryAsync<Tenant>(query);
                return result;
            }
        }

        public async Task<Tenant> Get(Guid id)
        {
            const string query = @"
                SELECT *
                FROM [Tenants]
                WHERE [Id] = @Id
            ";

            await using (var connecion = _dbConnectionFactory.GetDbConnection())
            {
                var result = await connecion.QuerySingleOrDefaultAsync<Tenant>(query, new { @Id = id });
                return result;
            }            
        }

        public async Task<Tenant> Create(CreateTenantDto createTenantDto)
        {
            const string query = @"
                INSERT INTO [Tenants]([Id], [Name])
                VALUES (@Id, @Name)
            ";
            
            await using (var connecion = _dbConnectionFactory.GetDbConnection())
            {
                var result = await connecion.ExecuteAsync(query, createTenantDto);
                var createdTenant = result == 0 ? null : _mapper.Map<Tenant>(createTenantDto);
                return createdTenant;
            }
        }

        public async Task<int> Update(UpdateTenantDto updateTenantDto)
        {
            const string query = @"
                UPDATE [Tenants]
                SET [Name] = @Name
                WHERE [Id] = @Id
            ";
            
            await using (var connecion = _dbConnectionFactory.GetDbConnection())
            {
                var result = await connecion.ExecuteAsync(query, updateTenantDto);
                return result;
            }
        }

        public async Task<int> Delete(DeleteTenantDto deleteTenantDto)
        {
            const string query = @"
                DELETE FROM [Tenants]
                WHERE [Id] = @Id
            ";
            
            await using (var connecion = _dbConnectionFactory.GetDbConnection())
            {
                var result = await connecion.ExecuteAsync(query, deleteTenantDto);
                return result;
            }
        }
    }
}