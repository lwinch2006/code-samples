using FluentValidation;

namespace Application.Logic.Tenants.Commands
{
    public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
    {
        public DeleteTenantCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}