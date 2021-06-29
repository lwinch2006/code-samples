using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Models.Tenants;
using Application.ViewModels.Tenants;
using AutoMapper;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Logic.Tenants.Queries
{
    public class GetTenantsQuery : IRequest<TenantsVm>
    {
        
    }
    
    public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, TenantsVm>
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;
        
        public GetTenantsQueryHandler(ITenantsRepository tenantsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }

        public async Task<TenantsVm> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
        {
            var tenantsDto = await _tenantsRepository.Get();
            var tenants = _mapper.Map<IEnumerable<Tenant>>(tenantsDto);
            var tenantsVm = new TenantsVm
            {
                Tenants = _mapper.Map<IEnumerable<TenantVm>>(tenants)
            };
            
            return tenantsVm;
        }
    }    
}