BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS(SELECT * FROM Information_Schema.Tables WHERE Table_Name='UserTokens')
        BEGIN
            CREATE TABLE [UserTokens] (
                [UserId] UNIQUEIDENTIFIER NOT NULL,
                [LoginProvider] VARCHAR(255) NOT NULL,
                [Name] VARCHAR(255) NOT NULL,
                [Value] VARCHAR(MAX),
                CONSTRAINT PK_UserToken PRIMARY KEY (UserId, LoginProvider, Name),
                CONSTRAINT FK_UserUserToken FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE, 
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