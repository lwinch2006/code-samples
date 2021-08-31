BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS [Users] 
(
     [Id] TEXT NOT NULL UNIQUE CONSTRAINT [PK_Users] PRIMARY KEY,

     [UserName] NVARCHAR(256) NOT NULL,
     [NormalizedUserName] NVARCHAR(256) NOT NULL CONSTRAINT [IX_Users_NormalizedUserName] UNIQUE,

     [Email] NVARCHAR(256) NOT NULL,
     [NormalizedEmail] NVARCHAR(256) NOT NULL CONSTRAINT [IX_Users_NormalizedEmail] UNIQUE,
     [EmailConfirmed] BIT NOT NULL,

     [PasswordHash] NVARCHAR(4000) NULL,
     [SecurityStamp] NVARCHAR(4000) NULL,
     [ConcurrencyStamp] NVARCHAR(4000) NOT NULL,

     [PhoneNumber] NVARCHAR(50) NULL,
     [PhoneNumberConfirmed] BIT NOT NULL,

     [TwoFactorEnabled] BIT NOT NULL,

     [LockoutEnd] DATETIME NULL,
     [LockoutEnabled] BIT NOT NULL,
     [AccessFailedCount] INT NOT NULL
);

COMMIT;