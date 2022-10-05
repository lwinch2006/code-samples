using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IdentityWithDapper.Infrastructure.Models.DTO.User;
using IdentityWithDapper.Infrastructure.Models.DTO.UserClaim;
using IdentityWithDapper.Infrastructure.Models.DTO.UserLogin;
using IdentityWithDapper.Infrastructure.Models.DTO.UserToken;
using IdentityWithDapper.Infrastructure.Repositories;
using IdentityWithDapper.Models;
using IdentityWithDapper.Utils.Constants;
using Microsoft.AspNetCore.Identity;

namespace IdentityWithDapper.Logic
{
    public class ApplicationUserStoreFull : 
        IUserPasswordStore<ApplicationUser>, 
        IUserEmailStore<ApplicationUser>, 
        IUserRoleStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>,
        IUserAuthenticatorKeyStore<ApplicationUser>,
        IUserTwoFactorRecoveryCodeStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>,
        IUserSecurityStampStore<ApplicationUser>,
        IQueryableUserStore<ApplicationUser>,
        IUserLoginStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IMapper _mapper;
        
        public IQueryable<ApplicationUser> Users => Get().GetAwaiter().GetResult();
        
        public ApplicationUserStoreFull(
            IUserRepository userRepository,
            IUserTokenRepository userTokenRepository,
            IUserClaimRepository userClaimRepository,
            IUserLoginRepository userLoginRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _userClaimRepository = userClaimRepository;
            _userLoginRepository = userLoginRepository;
            _mapper = mapper;
        }
        
        public void Dispose()
        {
        }

        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.Id.ToString());
        }

        public async Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.UserName = userName;
            
            return Task.CompletedTask;        }

        public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return await Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedUserName = normalizedName;
            
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createUserDto = _mapper.Map<CreateUserDto>(user);
            
            var result = await _userRepository.CreateAsync(createUserDto);

            return result == null
                ? IdentityResult.Failed()
                : IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var updateUserDto = _mapper.Map<UpdateUserDto>(user);

            var result = await _userRepository.UpdateAsync(updateUserDto);

            return result != 1
                ? IdentityResult.Failed()
                : IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var result = await _userRepository.DeleteAsync(user.Id);

            return result != 1
                ? IdentityResult.Failed()
                : IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(userId, out var userIdAsGuid))
            {
                return null;
            }

            var userDto = await _userRepository.FindByIdAsync(userIdAsGuid);

            var applicationUser = _mapper.Map<ApplicationUser>(userDto);

            return applicationUser;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = await _userRepository.FindByNameAsync(normalizedUserName);

            var applicationUser = _mapper.Map<ApplicationUser>(userDto);

            return applicationUser;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.PasswordHash);
        }

        public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Email = email;
            
            return Task.CompletedTask;
        }

        public async Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
         
            return await Task.FromResult(user.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.EmailConfirmed = confirmed;
            
            return Task.CompletedTask;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = await _userRepository.FindByEmailAsync(normalizedEmail);

            var applicationUser = _mapper.Map<ApplicationUser>(userDto);

            return applicationUser;
        }

        public async Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return await Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedEmail = normalizedEmail;
            
            return Task.CompletedTask;
        }

        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = _mapper.Map<UserDto>(user);
            
            await _userRepository.AddToRoleAsync(userDto, roleName);
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = _mapper.Map<UserDto>(user);
            
            await _userRepository.RemoveFromRoleAsync(userDto, roleName);            
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = _mapper.Map<UserDto>(user);

            var result = await _userRepository.GetRolesAsync(userDto);

            return result;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = _mapper.Map<UserDto>(user);

            var result = await _userRepository.IsInRoleAsync(userDto, roleName);

            return result;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var usersDto = await _userRepository.GetUsersInRoleAsync(roleName);

            var applicationUsers = _mapper.Map<IList<ApplicationUser>>(usersDto);

            return applicationUsers;
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneNumber = phoneNumber;
            
            return Task.CompletedTask;
        }

        public async Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.PhoneNumber);
        }

        public async Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneNumberConfirmed = confirmed;
            
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.TwoFactorEnabled = enabled;
            
            return Task.CompletedTask;
        }

        public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.TwoFactorEnabled);
        }

        public async Task SetAuthenticatorKeyAsync(ApplicationUser user, string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createUserTokenDto = new CreateOrUpdateUserTokenDto
            {
                UserId = user.Id,
                LoginProvider = ApplicationConstants.LoginProviderName,
                Name = ApplicationConstants.TwoFA.AuthenticatorKeyTokenName,
                Value = key
            };
            
            await _userTokenRepository.CreateOrUpdate(createUserTokenDto);
        }

        public async Task<string> GetAuthenticatorKeyAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var getUserTokenDto = new GetUserTokenDto
            {
                UserId = user.Id,
                LoginProvider = ApplicationConstants.LoginProviderName,
                Name = ApplicationConstants.TwoFA.AuthenticatorKeyTokenName
            };

            var userTokenDto = await _userTokenRepository.Get(getUserTokenDto);
            
            return userTokenDto?.Value;
        }

        public async Task ReplaceCodesAsync(ApplicationUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mergedCodes = string.Join(";", recoveryCodes);

            var createUserTokenDto = new CreateOrUpdateUserTokenDto
            {
                UserId = user.Id,
                LoginProvider = ApplicationConstants.LoginProviderName,
                Name = ApplicationConstants.TwoFA.RecoveryCodeTokenName,
                Value = mergedCodes
            };

            await _userTokenRepository.CreateOrUpdate(createUserTokenDto);
        }

        public async Task<bool> RedeemCodeAsync(ApplicationUser user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var getUserTokenDto = new GetUserTokenDto
            {
                UserId = user.Id,
                LoginProvider = ApplicationConstants.LoginProviderName,
                Name = ApplicationConstants.TwoFA.RecoveryCodeTokenName
            };
            
            var recoveryCodes = await _userTokenRepository.Get(getUserTokenDto);

            var recoveryCodesSplitted = recoveryCodes.Value.Split(";");

            if (!recoveryCodesSplitted.Contains(code))
            {
                return false;
            }

            var updatedRecoveryCodes = recoveryCodesSplitted.Where(t => t != code);

            await ReplaceCodesAsync(user, updatedRecoveryCodes, cancellationToken);

            return true;
        }

        public async Task<int> CountCodesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var getUserTokenDto = new GetUserTokenDto
            {
                UserId = user.Id,
                LoginProvider = ApplicationConstants.LoginProviderName,
                Name = ApplicationConstants.TwoFA.RecoveryCodeTokenName
            };
            
            var recoveryCodes = await _userTokenRepository.Get(getUserTokenDto);
            
            var recoveryCodesSplitted = (recoveryCodes?.Value ?? string.Empty).Split(";");

            return recoveryCodesSplitted.Length;
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var userClaimsDto = await _userClaimRepository.Get(user.Id);
            var claims = _mapper.Map<IList<Claim>>(userClaimsDto);
            return claims;
        }

        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createUserClaimsDto = claims.Select(t => new CreateUserClaimDto
            {
                ClaimType = t.Type,
                ClaimValue = t.Value,
                UserId = user.Id
            });

            await _userClaimRepository.Create(createUserClaimsDto);
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var updateUserClaimDto = new UpdateUserClaimDto
            {
                OldClaimType = claim.Type,
                OldClaimValue = claim.Value,
                NewClaimType = newClaim.Type,
                NewClaimValue = newClaim.Value,
                UserId = user.Id
            };

            await _userClaimRepository.Update(updateUserClaimDto);
        }

        public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var deleteUserClaimsDto = claims.Select(t => new DeleteUserClaimDto
            {
                ClaimType = t.Type,
                ClaimValue = t.Value,
                UserId = user.Id
            });
            
            await _userClaimRepository.Delete(deleteUserClaimsDto);
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var getUsersForClaimDto = new GetUsersForClaimDto
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };

            var usersDto = await _userRepository.GetUsersForClaim(getUsersForClaimDto);
            var applicationUsers = _mapper.Map<IList<ApplicationUser>>(usersDto);
            return applicationUsers;
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public async Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.SecurityStamp);
        }
        
        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var createUserLoginDto = new CreateUserLoginDto
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName,
                UserId = user.Id
            };

            await _userLoginRepository.Create(createUserLoginDto);
        }

        public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var deleteUserLoginDto = new DeleteUserLoginDto
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
                UserId = user.Id
            };

            await _userLoginRepository.Delete(deleteUserLoginDto);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userLoginsDto = await _userLoginRepository.Get(user.Id);
            var userLoginInfoList = userLoginsDto.Select(t => new UserLoginInfo(t.LoginProvider, t.ProviderKey, t.ProviderDisplayName)).ToList();
            return userLoginInfoList;
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var userDto = await _userRepository.FindByLogin(loginProvider, providerKey);
            var applicationUser = _mapper.Map<ApplicationUser>(userDto);
            return applicationUser;
        }

        public async Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.LockoutEnd = lockoutEnd;
            
            return Task.CompletedTask;
        }

        public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.AccessFailedCount++;

            return await Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            user.AccessFailedCount = 0;
            
            return Task.CompletedTask;
        }

        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.AccessFailedCount);
        }

        public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.LockoutEnabled = enabled;

            return Task.CompletedTask;
        }
        
        private async Task<IQueryable<ApplicationUser>> Get(CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var usersDto = await _userRepository.Get();
            
            var applicationUsers = _mapper.Map<IEnumerable<ApplicationUser>>(usersDto);

            return applicationUsers.AsQueryable();
        }
        
    }
}