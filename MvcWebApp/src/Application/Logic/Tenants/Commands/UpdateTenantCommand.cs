using System.Threading;
using Application.Logic.ServiceBus;
using Application.Models.ServiceBus.Tenants.V1;
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
        private readonly IApplicationServiceBusClient _applicationServiceBusClient;
        private readonly IMapper _mapper;
        
        public UpdateTenantCommandHandler(
            ITenantsRepository tenantsRepository,
            IApplicationServiceBusClient applicationServiceBusClient,
            IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _applicationServiceBusClient = applicationServiceBusClient;
            _mapper = mapper;
        }
        
        public async Task<int> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            // Updating tenant
            var updateRequestDto = _mapper.Map<UpdateTenantDto>(request);
            var result = await _tenantsRepository.Update(updateRequestDto);
            
            // Sending event
            var updatedEvent = _mapper.Map<TenantUpdatedEvent>(updateRequestDto);
            await _applicationServiceBusClient.SendTenantUpdatedEvent(
                updatedEvent,
                nameof(UpdateTenantCommandHandler), 
                cancellationToken);
            
            return result;
        }
    }
}