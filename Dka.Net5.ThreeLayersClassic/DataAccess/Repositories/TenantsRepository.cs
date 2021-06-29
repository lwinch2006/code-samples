using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DataAccess.DTO.Tenants;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;

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
    private readonly string _connectionString;

    public TenantsRepository(IConfiguration configuration)
    {
        _connectionString = configuration["Data:ConnectionString"];
    }

    public async Task<IEnumerable<Tenant>> Get()
    {
        const string query = @"
                SELECT *
                FROM [Tenants]
            ";

        await using (var connecion = new SqlConnection(_connectionString))
        {
            var result = await connecion.QueryAsync<Tenant>(query);
            return result;
        }
    }

    public async Task<Tenant> Get(Guid id)
    {
        return await Task.FromResult(new Tenant());
    }

    public async Task<Tenant> Create(CreateTenantDto createTenantDto)
    {
        return await Task.FromResult(new Tenant());
    }

    public async Task<int> Update(UpdateTenantDto updateTenantDto)
    {
        return await Task.FromResult(1);
    }

    public async Task<int> Delete(DeleteTenantDto deleteTenantDto)
    {
        return await Task.FromResult(1);
    }
}