﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.DTO.User;
using Dka.Net5.IdentityWithDapper.Infrastructure.Models.Entities;
using Dka.Net5.IdentityWithDapper.Infrastructure.Utils;
using Dka.Net5.IdentityWithDapper.Infrastructure.Utils.Constants;
using Dka.Net5.IdentityWithDapper.Utils.Constants;
using Microsoft.Extensions.Configuration;

namespace Dka.Net5.IdentityWithDapper.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<int> UpdateAsync(UpdateUserDto updateUserDto);
        Task<int> DeleteAsync(Guid userId);
        Task<UserDto> FindByIdAsync(Guid userId);
        Task<UserDto> FindByNameAsync(string normalizedUserName);
        Task<UserDto> FindByEmailAsync(string normalizedEmail);
        Task<UserDto> FindByLogin(string loginProvider, string providerKey);
        Task AddToRoleAsync(UserDto userDto, string roleName);
        Task RemoveFromRoleAsync(UserDto userDto, string roleName);
        Task<IList<string>> GetRolesAsync(UserDto userDto);
        Task<bool> IsInRoleAsync(UserDto userDto, string roleName);
        Task<IList<UserDto>> GetUsersInRoleAsync(string roleName);
        Task<IList<UserDto>> GetUsersForClaim(GetUsersForClaimDto getUsersForClaimDto);
        Task<IEnumerable<UserDto>> Get();
    }
    
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;

        public UserRepository(
            IDbConnectionFactory dbConnectionFactory,
            IMapper mapper)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _mapper = mapper;
        }
        
        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            const string query = @"
                INSERT INTO [Users] (
                    [Id]
                    ,[UserName]
                    ,[NormalizedUserName]
                    ,[Email]
                    ,[NormalizedEmail]
                    ,[EmailConfirmed]
                    ,[PasswordHash]
                    ,[SecurityStamp]
                    ,[ConcurrencyStamp]
                    ,[PhoneNumber]
                    ,[PhoneNumberConfirmed]
                    ,[TwoFactorEnabled]
                    ,[LockoutEnd]
                    ,[LockoutEnabled]
                    ,[AccessFailedCount])
                VALUES (
                    @Id
                    ,@UserName
                    ,@NormalizedUserName
                    ,@Email
                    ,@NormalizedEmail
                    ,@EmailConfirmed
                    ,@PasswordHash
                    ,@SecurityStamp
                    ,@ConcurrencyStamp
                    ,@PhoneNumber
                    ,@PhoneNumberConfirmed
                    ,@TwoFactorEnabled
                    ,@LockoutEnd
                    ,@LockoutEnabled
                    ,@AccessFailedCount);
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var insertedRowsNum = await connection.ExecuteAsync(query, createUserDto);

                if (insertedRowsNum == 0)
                {
                    return null;
                }

                var userDto = _mapper.Map<UserDto>(createUserDto);

                return userDto;
            }
        }

        public async Task<int> UpdateAsync(UpdateUserDto updateUserDto)
        {
            const string query = @"
                UPDATE [Users]
                SET 
                    [UserName] = @UserName
                    ,[NormalizedUserName] = @NormalizedUserName
                    ,[Email] = @Email
                    ,[NormalizedEmail] = @NormalizedEmail
                    ,[EmailConfirmed] = @EmailConfirmed
                    ,[PasswordHash] = @PasswordHash
                    ,[SecurityStamp] = @SecurityStamp
                    ,[ConcurrencyStamp] = @ConcurrencyStamp
                    ,[PhoneNumber] = @PhoneNumber
                    ,[PhoneNumberConfirmed] = @PhoneNumberConfirmed
                    ,[TwoFactorEnabled] = @TwoFactorEnabled
                    ,[LockoutEnd] = @LockoutEnd
                    ,[LockoutEnabled] = @LockoutEnabled
                    ,[AccessFailedCount] = @AccessFailedCount
                WHERE [Id] = @Id;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var updatedRowsNum = await connection.ExecuteAsync(query, updateUserDto);

                return updatedRowsNum;
            }
        }

        public async Task<int> DeleteAsync(Guid userId)
        {
            const string query = @"
                DELETE
                FROM [Users]
                WHERE [Id] = @Id;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var deletedRowsNum = await connection.ExecuteAsync(query, new {@Id = userId});

                return deletedRowsNum;
            }
        }

        public async Task<UserDto> FindByIdAsync(Guid userId)
        {
            const string query = @"
                SELECT *
                FROM [Users]
                WHERE [Id] = @Id;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new {@Id = userId});

                var userDto = _mapper.Map<UserDto>(user);

                return userDto;
            }
        }

        public async Task<UserDto> FindByNameAsync(string normalizedUserName)
        {
            const string query = @"
                SELECT *
                FROM [Users]
                WHERE [NormalizedUserName] = @NormalizedUserName;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new {@NormalizedUserName = normalizedUserName});

                var userDto = _mapper.Map<UserDto>(user);

                return userDto;
            }
        }

        public async Task<UserDto> FindByEmailAsync(string normalizedEmail)
        {
            const string query = @"
                SELECT *
                FROM [Users]
                WHERE [NormalizedEmail] = @NormalizedEmail;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new {@NormalizedEmail = normalizedEmail});
                var userDto = _mapper.Map<UserDto>(user);
                return userDto;
            }
        }

        public async Task<UserDto> FindByLogin(string loginProvider, string providerKey)
        {
            const string query = @"
                SELECT [u].*
                FROM [Users] [u] INNER JOIN [UserLogins] [ul] on [u].[Id] = [ul].[UserId]
                WHERE [ul].[LoginProvider] = @LoginProvider
                AND [ul].[ProviderKey] = @ProviderKey;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { @LoginProvider = loginProvider, @ProviderKey = providerKey });
                var userDto = _mapper.Map<UserDto>(user);
                return userDto;
            }
        }

        public async Task AddToRoleAsync(UserDto userDto, string roleName)
        {
            const string query = @"
                DECLARE @RoleId INT = 0;

                SELECT @RoleId = [Id]
                FROM [Roles]
                WHERE [Name] = @RoleName

                IF (@RoleId > 0)
                BEGIN
                    INSERT INTO [UserRoles] ([UserId], [RoleId])
                    VALUES (@UserId, @RoleId);
                END;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = _mapper.Map<User>(userDto);

                var insertedRowsNum = await connection.ExecuteAsync(query, new {@UserId = user.Id, @RoleName = roleName});
            }
        }

        public async Task RemoveFromRoleAsync(UserDto userDto, string roleName)
        {
            const string query = @"
                DECLARE @RoleId INT = 0;

                SELECT @RoleId = [Id]
                FROM [Roles]
                WHERE [Name] = @RoleName

                IF (@RoleId > 0)
                BEGIN
                    DELETE FROM [UserRoles]
                    WHERE [UserId] = @UserId 
                    AND [RoleId] = @RoleId;
                END;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = _mapper.Map<User>(userDto);

                var deletedRowsNum = await connection.ExecuteAsync(query, new {@UserId = user.Id, @RoleName = roleName});
            }
        }

        public async Task<IList<string>> GetRolesAsync(UserDto userDto)
        {
            const string query = @"
                SELECT [r].[Name]
                FROM [Roles] [r] INNER JOIN [UserRoles] [ur] ON [r].[Id] = [ur].[RoleId]
                WHERE [ur].[UserId] = @UserId;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = _mapper.Map<User>(userDto);
                var rolesNames = await connection.QueryAsync<string>(query, new {@UserId = user.Id});
                return (IList<string>)rolesNames;
            }
        }

        public async Task<bool> IsInRoleAsync(UserDto userDto, string roleName)
        {
            const string query = @"
                SELECT COUNT(1)
                FROM [UserRoles] [ur] INNER JOIN [Roles] [r] ON [ur].[RoleId] = [r].[Id]
                WHERE [ur].[UserId] = @UserId
                AND [r].[Name] = @RoleName;
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var user = _mapper.Map<User>(userDto);
                
                var isInRole = await connection.ExecuteScalarAsync<bool>(query, new {@UserId = user.Id, @RoleName = roleName});

                return isInRole;
            }
        }

        public async Task<IList<UserDto>> GetUsersInRoleAsync(string roleName)
        {
            const string query = @"
                SELECT *
                FROM [Users] [u] INNER JOIN [UserRoles] [ur] ON [u].[Id] = [ur].[UserId]
                WHERE [ur].[RoleId] IN (SELECT [r].[Id] FROM [Roles] [r] WHERE [r].[Name] = @RoleName);
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var users = await connection.QueryAsync<User>(query, new {@RoleName = roleName});
                var usersDto = _mapper.Map<IList<UserDto>>(users);
                return usersDto;
            }
        }

        public async Task<IList<UserDto>> GetUsersForClaim(GetUsersForClaimDto getUsersForClaimDto)
        {
            const string query = @"
                SELECT [u].*
                FROM [Users] [u] INNER JOIN [UserClaims] [uc] ON [u].[Id] = [uc].[UserId]
                WHERE [uc].[ClaimType] = @ClaimType
                AND [uc].[ClaimValue] = @ClaimValue
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var users = await connection.QueryAsync<User>(query, getUsersForClaimDto);
                var usersDto = _mapper.Map<IList<UserDto>>(users);
                return usersDto;
            }
        }

        public async Task<IEnumerable<UserDto>> Get()
        {
            const string query = @"
                SELECT *
                FROM [Users]
            ";

            using (var connection = _dbConnectionFactory.GetDbConnection())
            {
                var users = await connection.QueryAsync<User>(query);
                var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
                return usersDto;
            }
        }
    }
}