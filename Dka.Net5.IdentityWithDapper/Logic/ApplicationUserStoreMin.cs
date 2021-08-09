using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.UserToken;
using Dka.Net5.IdentityWithDapper.Infrastructure.Repositories;
using Dka.Net5.IdentityWithDapper.Models;
using Dka.Net5.IdentityWithDapper.Utils.Constants;
using Microsoft.AspNetCore.Identity;

namespace Dka.Net5.IdentityWithDapper.Logic
{
    public class ApplicationUserStoreMin : 
        IUserPasswordStore<ApplicationUser>, 
        IUserEmailStore<ApplicationUser>, 
        IUserRoleStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>,
        IUserAuthenticatorKeyStore<ApplicationUser>,
        IUserTwoFactorRecoveryCodeStore<ApplicationUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IMapper _mapper;
        
        public ApplicationUserStoreMin(
            IUserRepository userRepository,
            IUserTokenRepository userTokenRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
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
                LoginProvider = SystemConstants.LoginProviderName,
                Name = SystemConstants.TwoFA.AuthenticatorKeyTokenName,
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
                LoginProvider = SystemConstants.LoginProviderName,
                Name = SystemConstants.TwoFA.AuthenticatorKeyTokenName
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
                LoginProvider = SystemConstants.LoginProviderName,
                Name = SystemConstants.TwoFA.RecoveryCodeTokenName,
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
                LoginProvider = SystemConstants.LoginProviderName,
                Name = SystemConstants.TwoFA.RecoveryCodeTokenName
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
                LoginProvider = SystemConstants.LoginProviderName,
                Name = SystemConstants.TwoFA.RecoveryCodeTokenName
            };
            
            var recoveryCodes = await _userTokenRepository.Get(getUserTokenDto);
            
            var recoveryCodesSplitted = (recoveryCodes?.Value ?? string.Empty).Split(";");

            return recoveryCodesSplitted.Length;
        }
    }
}