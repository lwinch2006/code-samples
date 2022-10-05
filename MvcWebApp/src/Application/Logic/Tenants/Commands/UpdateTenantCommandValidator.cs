using FluentValidation;

namespace Application.Logic.Tenants.Commands
{
    public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
    {
        public UpdateTenantCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MinimumLength(3);
        }
    }
}