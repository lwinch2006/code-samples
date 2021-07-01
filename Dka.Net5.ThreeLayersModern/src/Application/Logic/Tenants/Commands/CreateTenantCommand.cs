using System.Threading;
using System.Threading.Tasks;
using Application.Models.Tenants;
using Application.ViewModels.Tenants;
using AutoMapper;
using Infrastructure.DTO.Tenants;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Logic.Tenants.Commands
{
    public class CreateTenantCommand : IRequest<TenantVm>
    {
        public string Name { get; set; }
    }

    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantVm>
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;
        
        public CreateTenantCommandHandler(ITenantsRepository tenantsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }        
        
        public async Task<TenantVm> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var createRequestDto = _mapper.Map<CreateTenantDto>(request);
            var tenantDto = await _tenantsRepository.Create(createRequestDto);
            var tenant = _mapper.Map<Tenant>(tenantDto);
            var tenantVm = _mapper.Map<TenantVm>(tenant);
            return tenantVm;
        }
    }
}