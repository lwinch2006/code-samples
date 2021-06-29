using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogic.DTO.Tenants;
using BusinessLogic.Models.Tenants;

namespace BusinessLogic.Logic
{
    public interface ITenantsLogic
    {
        Task<IEnumerable<Tenant>> Get();

        Task<Tenant> Get(Guid id);

        Task<Tenant> Create(CreateTenantDto createTenantDto);

        Task<int> Update(UpdateTenantDto updateTenantDto);

        Task<int> Delete(DeleteTenantDto deleteTenantDto);
    }
    
    public class TenantsLogic : ITenantsLogic
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;

        public TenantsLogic(ITenantsRepository tenantsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Tenant>> Get()
        {
            var tenantsDto = await _tenantsRepository.Get();
            var tenants = _mapper.Map<IEnumerable<Tenant>>(tenantsDto);
            return tenants;
        }

        public async Task<Tenant> Get(Guid id)
        {
            var tenantDto = await _tenantsRepository.Get(id);
            var tenant = _mapper.Map<Tenant>(tenantDto);
            return tenant;
        }

        public async Task<Tenant> Create(CreateTenantDto createTenantDto)
        {
            var createTenantRequest = _mapper.Map<DataAccess.DTO.Tenants.CreateTenantDto>(createTenantDto);
            var tenantDto = await _tenantsRepository.Create(createTenantRequest);
            var tenant = _mapper.Map<Tenant>(tenantDto);
            return tenant;
        }

        public Task<int> Update(UpdateTenantDto updateTenantDto)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(DeleteTenantDto deleteTenantDto)
        {
            throw new NotImplementedException();
        }
    }
}