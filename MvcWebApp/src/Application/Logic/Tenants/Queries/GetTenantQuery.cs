using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Models.Tenants;
using Application.ViewModels.Tenants;
using AutoMapper;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Logic.Tenants.Queries
{
    public class GetTenantQuery : IRequest<TenantVm>
    {
        public Guid Id { get; set; }
    }

    public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, TenantVm>
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;
        
        public GetTenantQueryHandler(ITenantsRepository tenantsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }        
        
        public async Task<TenantVm> Handle(GetTenantQuery request, CancellationToken cancellationToken)
        {
            var tenantsDto = await _tenantsRepository.Get(request.Id);
            var tenant = _mapper.Map<Tenant>(tenantsDto);
            var tenantVm = _mapper.Map<TenantVm>(tenant);
            
            return tenantVm;            
        }
    }
}