BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS [UserRoles] 
(
    [UserId] VARCHAR(36) NOT NULL,
    [RoleId] VARCHAR(36) NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserUserRole FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_RoleUserRole FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON UPDATE CASCADE ON DELETE CASCADE
);

COMMIT;