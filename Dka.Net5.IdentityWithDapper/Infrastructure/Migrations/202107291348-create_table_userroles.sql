BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS(SELECT * FROM Information_Schema.Tables WHERE Table_Name='UserRoles')
        BEGIN
            CREATE TABLE [UserRoles] (
                [UserId] UNIQUEIDENTIFIER NOT NULL,
                [RoleId] UNIQUEIDENTIFIER NOT NULL,
                CONSTRAINT PK_UserRole PRIMARY KEY (UserId, RoleId),
                CONSTRAINT FK_UserUserRole FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE CASCADE, 
                CONSTRAINT FK_RoleUserRole FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON UPDATE CASCADE ON DELETE CASCADE, 
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