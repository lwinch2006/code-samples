BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS [UserLogins] 
(
    [LoginProvider] VARCHAR(255) NOT NULL,
    [ProviderKey] VARCHAR(255) NOT NULL,
    [ProviderDisplayName] VARCHAR(255),
    [UserId] VARCHAR(36) NOT NULL,
    CONSTRAINT PK_UserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_UserUserLogin FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE
);

COMMIT;