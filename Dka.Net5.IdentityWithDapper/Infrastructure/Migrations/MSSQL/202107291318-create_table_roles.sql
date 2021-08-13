BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS(SELECT * FROM Information_Schema.Tables WHERE Table_Name='Roles')
        BEGIN
            CREATE TABLE [Roles] (
                [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED,
                [Name] NVARCHAR(256) NOT NULL,
                [NormalizedName] NVARCHAR(256) NOT NULL CONSTRAINT [IX_Roles_NormalizedName] UNIQUE NONCLUSTERED,
                [ConcurrencyStamp] NVARCHAR(max) NOT NULL
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