BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS [UserTokens] 
(
    [UserId] VARCHAR(36) NOT NULL,
    [LoginProvider] VARCHAR(255) NOT NULL,
    [Name] VARCHAR(255) NOT NULL,
    [Value] VARCHAR(4000),
    CONSTRAINT PK_UserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_UserUserToken FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE
);

COMMIT;