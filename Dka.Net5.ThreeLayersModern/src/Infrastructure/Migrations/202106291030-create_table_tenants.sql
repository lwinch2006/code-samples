BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS(SELECT * FROM Information_Schema.Tables WHERE Table_Name='Tenants')
        BEGIN
            CREATE TABLE Tenants
            (
                Id UNIQUEIDENTIFIER NOT NULL,
                Name NVARCHAR(255) NOT NULL,
                CONSTRAINT PK_Tenant PRIMARY KEY (Id)
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