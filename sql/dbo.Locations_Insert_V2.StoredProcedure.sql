USE [C84_InternsLA]
GO
/****** Object:  StoredProcedure [dbo].[Locations_Insert_V2]    Script Date: 4/9/2020 8:08:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Locations_Insert_V2] @LocationTypeId INT, 
                                       @LineOne        NVARCHAR(255), 
                                       @LineTwo        NVARCHAR(255), 
                                       @City           NVARCHAR(255), 
                                       @Zip            NVARCHAR(50), 
                                       @StateId        INT, 
                                       @Latitude       FLOAT, 
                                       @Longitude      FLOAT, 
                                       @CreatedBy      INT, 
                                       @ModifiedBy     INT, 
                                       @Id             INT OUT

/* TEST CODE
 
DECLARE @_Id int = 0,
		@_LocationTypeId int = 1,
		@_LineOne nvarchar(255) = '420 High St.',
		@_LineTwo nvarchar(255) = 'Apt. 420',
		@_City nvarchar(255) = 'Mary Jane',
		@_Zip nvarchar(50) = '01295',
		@_StateId int = 1,
		@_Latitude float = 61.22,
		@_Longitude float = 77.25, 
		@_CreatedBy int = 1,
		@_ModifiedBy int = 1

		
 
EXEC dbo.Locations_Insert 	
		@_LocationTypeId,
		@_LineOne,
		@_LineTwo,
		@_City,
		@_Zip,
		@_StateId,
		@_Latitude,
		@_Longitude,
		@_CreatedBy,
		@_ModifiedBy,
		@_Id OUTPUT

SELECT * FROM dbo.Locations


*/

AS
     SET XACT_ABORT ON;
     DECLARE @Tran NVARCHAR(50)= '_locationInsert';
    BEGIN TRY
        BEGIN TRANSACTION @Tran;
        BEGIN
            DECLARE @DateModified DATETIME2(7)= GETUTCDATE(), @SRID INT= 4326;
            DECLARE @GeoPoint GEOGRAPHY= GEOGRAPHY ::Point(@Latitude, @Longitude, @SRID);
            INSERT INTO dbo.Locations
            (LocationTypeId, 
             LineOne, 
             LineTwo, 
             City, 
             Zip, 
             StateId, 
             Latitude, 
             Longitude, 
             CreatedBy, 
             ModifiedBy, 
             GeoPoint
            )
            VALUES
            (@LocationTypeId, 
             @LineOne, 
             @LineTwo, 
             @City, 
             @Zip, 
             @StateId, 
             @Latitude, 
             @Longitude, 
             @CreatedBy, 
             @ModifiedBy, 
             @GeoPoint
            );
            SET @Id = SCOPE_IDENTITY();
        END;
        COMMIT TRANSACTION @Tran;
    END TRY
    BEGIN CATCH
        IF(XACT_STATE()) = -1
            BEGIN
                PRINT 'The transaction is in an uncommittable state.' + ' Rolling back transaction.';
                ROLLBACK TRANSACTION @Tran;
        END;

        -- Test whether the transaction is active and valid.  
        IF(XACT_STATE()) = 1
            BEGIN
                PRINT 'The transaction is committable.' + ' Committing transaction.';
                COMMIT TRANSACTION @Tran;
        END;

        -- If you want to see error info
        -- SELECT
        --ERROR_NUMBER() AS ErrorNumber,
        --ERROR_SEVERITY() AS ErrorSeverity,
        --ERROR_STATE() AS ErrorState,
        -- ERROR_PROCEDURE() AS ErrorProcedure,
        -- ERROR_LINE() AS ErrorLine,
        -- ERROR_MESSAGE() AS ErrorMessage
        -- to just get the error thrown and see the bad news as an exception
        THROW;
    END CATCH;
     SET XACT_ABORT OFF;
GO
