BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS [RoleClaims] 
(
    [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RoleClaims] PRIMARY KEY,
    [ClaimType] VARCHAR(255) NOT NULL,
    [ClaimValue] VARCHAR(255),
    [RoleId] VARCHAR(36) NOT NULL,
    CONSTRAINT FK_RoleRoleClaim FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON UPDATE CASCADE ON DELETE CASCADE
);
    
COMMIT;