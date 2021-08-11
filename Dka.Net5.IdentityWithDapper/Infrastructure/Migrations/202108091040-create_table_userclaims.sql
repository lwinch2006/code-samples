BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS(SELECT * FROM Information_Schema.Tables WHERE Table_Name='UserClaims')
        BEGIN
            CREATE TABLE [UserClaims] (
                [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserClaims] PRIMARY KEY CLUSTERED,
                [ClaimType] VARCHAR(255) NOT NULL,
                [ClaimValue] VARCHAR(255),
                [UserId] UNIQUEIDENTIFIER NOT NULL,
                CONSTRAINT FK_UserUserClaim FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE 
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