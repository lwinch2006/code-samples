BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS(SELECT * FROM Information_Schema.Tables WHERE Table_Name='Users')
        BEGIN
            CREATE TABLE [Users] (
                [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED,
                
                [UserName] NVARCHAR(256) NOT NULL,
                [NormalizedUserName] NVARCHAR(256) NOT NULL CONSTRAINT [IX_Users_NormalizedUserName] UNIQUE NONCLUSTERED,
                
                [Email] NVARCHAR(256) NOT NULL,
                [NormalizedEmail] NVARCHAR(256) NOT NULL CONSTRAINT [IX_Users_NormalizedEmail] UNIQUE NONCLUSTERED,
                [EmailConfirmed] BIT NOT NULL,

                [PasswordHash] NVARCHAR(max) NOT NULL,
                [SecurityStamp] NVARCHAR(max) NULL,
                [ConcurrencyStamp] NVARCHAR(max) NOT NULL,
                
                [PhoneNumber] NVARCHAR(50) NULL,
                [PhoneNumberConfirmed] BIT NOT NULL,

                [TwoFactorEnabled] BIT NOT NULL,

                [LockoutEnd] DATETIME NULL,                
                [LockoutEnabled] BIT NOT NULL,
                [AccessFailedCount] INT NOT NULL
            );
        END;

    IF @@TRANCOUNT > 0
        BEGIN
            COMMIT TRANSACTION;
        END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

    DECLARE @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    SELECT @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;