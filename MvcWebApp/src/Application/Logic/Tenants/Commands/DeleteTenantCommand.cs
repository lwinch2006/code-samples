using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.DTO.Tenants;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Logic.Tenants.Commands
{
    public class DeleteTenantCommand : IRequest<int>
    {
        public Guid Id { get; init; }
    }

    public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, int>
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IMapper _mapper;
        
        public DeleteTenantCommandHandler(ITenantsRepository tenantsRepository, IMapper mapper)
        {
            _tenantsRepository = tenantsRepository;
            _mapper = mapper;
        }         
        
        public async Task<int> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var deleteRequestDto = _mapper.Map<DeleteTenantDto>(request);
            var result = await _tenantsRepository.Delete(deleteRequestDto);
            return result;
        }
    }
}