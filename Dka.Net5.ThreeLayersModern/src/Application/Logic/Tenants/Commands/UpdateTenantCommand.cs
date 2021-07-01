using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.DTO.Tenants;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Logic.Tenants.Commands
{
    public class UpdateTenantCommand : IRequest<int>
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
    }

    public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, int>
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;
        
        public UpdateTenantCommandHandler(ITenantsRepository tenantsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }
        
        public async Task<int> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var updateRequestDto = _mapper.Map<UpdateTenantDto>(request);
            var result = await _tenantsRepository.Update(updateRequestDto);
            return result;
        }
    }
}