BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS(SELECT * FROM [Information_Schema].[Tables] WHERE Table_Name='Tenants')
        BEGIN
            INSERT INTO [Tenants] ([Id], [Name])
            VALUES
            (NEWID(), 'Umbrella Corporation'),
            (NEWID(), 'Cyberdyne Systems'),
            (NEWID(), 'OCP');
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