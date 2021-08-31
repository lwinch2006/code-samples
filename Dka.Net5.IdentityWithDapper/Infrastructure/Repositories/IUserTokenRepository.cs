using System.Threading.Tasks;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserToken;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserTokenRepository
    {
        Task<UserTokenDto> CreateOrUpdate(CreateOrUpdateUserTokenDto createOrUpdateUserTokenDto);
        Task<UserTokenDto> Get(GetUserTokenDto getUserTokenDto);
    }
}