using System.Threading.Tasks;
using IdentityWithDapper.Infrastructure.Models.DTO.UserToken;

namespace IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserTokenRepository
    {
        Task<UserTokenDto> CreateOrUpdate(CreateOrUpdateUserTokenDto createOrUpdateUserTokenDto);
        Task<UserTokenDto> Get(GetUserTokenDto getUserTokenDto);
    }
}