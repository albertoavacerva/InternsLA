USE [C84_InternsLA]
GO
/****** Object:  StoredProcedure [dbo].[Events_Update_V3]    Script Date: 4/9/2020 8:09:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[Events_Update_V3] @EventId          INT, 
                                     @EventTypeId      INT, 
                                     @Name             NVARCHAR(255), 
                                     @Summary          NVARCHAR(255), 
                                     @ShortDescription NVARCHAR(4000), 
                                     @EventStatusId    INT, 
                                     @ImageUrl         NVARCHAR(400), 
                                     @ExternalSiteUrl  NVARCHAR(400), 
                                     @IsFree           BIT, 
                                     @EventFiles AS       FILETABLETYPE READONLY, 
                                     @VenueId          INT, 
                                     @VenueName        NVARCHAR(255), 
                                     @VenueDescription NVARCHAR(4000), 
                                     @VenueUrl         NVARCHAR(255), 
                                     @VenueFiles AS       FILETABLETYPE READONLY, 
                                     @LocationId       INT, 
                                     @LocationTypeId   INT, 
                                     @LocationLineOne  NVARCHAR(255), 
                                     @LocationLineTwo  NVARCHAR(255), 
                                     @LocationCity     NVARCHAR(225), 
                                     @LocationZip      NVARCHAR(50), 
                                     @LocationStateId  INT, 
                                     @LocationLat      FLOAT, 
                                     @LocationLong     FLOAT, 
                                     @DateStart        DATETIME2(7), 
                                     @DateEnd          DATETIME2(7), 
                                     @CreatedBy        INT, 
                                     @ModifiedBy       INT
AS

/*
DECLARE 

	@EventId INT = 32,
	@EventTypeId INT= 1,
	@Name NVARCHAR(255)= 'Anthony Surprise Party', 
	@Summary NVARCHAR(255)= 'Grad party', 
	@ShortDescription NVARCHAR(4000)= 'Party', 
	@EventStatusId INT= 1, 
	@ImageUrl NVARCHAR(400)= 'www.lavb.com', 
	@ExternalSiteUrl NVARCHAR(400)= 'www.la.com', 
	@IsFree INT= 1, 
	@VenueId INT= 106, 
	@VenueName NVARCHAR(255)= 'Boom Bap', 
	@VenueDescription NVARCHAR(4000)= 'Description', 
	@VenueUrl NVARCHAR(255)= 'www.323.com', 
	@LocationId INT= 0, 
	@LocationTypeId INT= 2, 
	@LocationLineOne NVARCHAR(255)= '456 Bang', 
	@LocationLineTwo NVARCHAR(255), 
	@LocationCity NVARCHAR(225)= 'Anaheim', 
	@LocationZip NVARCHAR(50)= '90033', 
	@LocationStateId INT= 9, 
	@LocationLat FLOAT= 34.0407, 
	@LocationLong FLOAT= -118.2468, 
	@DateStart DATETIME2(7)= GETUTCDATE(), 
	@DateEnd DATETIME2(7)= GETUTCDATE(), 
	@CreatedBy INT= 8, 
	@ModifiedBy INT= 8; 


DECLARE @EventFiles AS FILETABLETYPE;
INSERT INTO @EventFiles
([Url], 
 [FileTypeId], 
 [CreatedBy]
)
VALUES ('www.123fdsaffsadf.come', 2, @CreatedBy), ('www.4aw23j4k2m56.come', 2, @CreatedBy)

DECLARE @VenueFiles AS FILETABLETYPE;
INSERT INTO @VenueFiles
([Url], 
 [FileTypeId], 
 [CreatedBy]
)
       SELECT [Url], 
              [FileTypeId], 
              [CreatedBy]
       FROM dbo.[Files]
       ORDER BY [Url]
       OFFSET (5) ROWS FETCH NEXT 5 ROWS ONLY;

SELECT *
FROM DBO.[Events] AS E
JOIN DBO.Venues AS V
ON E.VenueId = V.Id
JOIN DBO.Locations AS L
ON V.LocationId = L.Id
WHERE @EventId = E.Id;

EXECUTE [dbo].[Events_Update_V3] 

        @EventId,
	    @EventTypeId, 
        @Name, 
        @Summary, 
        @ShortDescription, 
        @EventStatusId, 
        @ImageUrl, 
        @ExternalSiteUrl, 
        @IsFree, 
        @EventFiles, 
        @VenueId, 
        @VenueName, 
        @VenueDescription, 
        @VenueUrl, 
        @VenueFiles, 
        @LocationId, 
        @LocationTypeId, 
        @LocationLineOne, 
        @LocationLineTwo, 
        @LocationCity, 
        @LocationZip, 
        @LocationStateId, 
        @LocationLat, 
        @LocationLong, 
        @DateStart, 
        @DateEnd, 
        @CreatedBy, 
        @ModifiedBy
        
SELECT *
FROM DBO.[Events] AS E
JOIN DBO.Venues AS V
ON E.VenueId = V.Id
JOIN DBO.Locations AS L
ON V.LocationId = L.Id
WHERE @EventId = E.Id;

*/

    BEGIN
        SET XACT_ABORT ON;
        DECLARE @Tran NVARCHAR(50)= 'Event_Update_Tx';
        BEGIN TRY
            BEGIN TRANSACTION @Tran;

            --- IF LOCATIONID = 0 ---
            IF @LocationId = 0
                BEGIN
                    --- INSERT INTO LOCATION TABLE ---

                    EXEC dbo.Locations_Insert_V2 
                         @LocationTypeId, 
                         @LocationLineOne, 
                         @LocationLineTwo, 
                         @LocationCity, 
                         @LocationZip, 
                         @LocationStateId, 
                         @LocationLat, 
                         @LocationLong, 
                         @CreatedBy, 
                         @ModifiedBy, 
                         @LocationId OUTPUT;
            END;
            --- IF VENUEID = 0 ---
            IF @VenueId = 0
                BEGIN
                    --- INSERT INTO VENUE TABLE ---
                    EXEC dbo.Venues_Insert_V2 
                         @VenueId OUTPUT, 
                         @VenueName, 
                         @VenueDescription, 
                         @LocationId, 
                         @VenueUrl, 
                         @ModifiedBy, 
                         @CreatedBy, 
                         @VenueFiles;
            END;
            --- UPDATE EVENTS TABLE ---
            EXEC dbo.Events_Update_V2 
                 @EventId, 
                 @EventTypeId, 
                 @Name, 
                 @Summary, 
                 @ShortDescription, 
                 @VenueId, 
                 @EventStatusId, 
                 @ImageUrl, 
                 @ExternalSiteUrl, 
                 @IsFree, 
                 @DateStart, 
                 @DateEnd, 
                 @CreatedBy;

            --- INSERT EVENT FILES INTO FILES TABLE ---
            DECLARE @NewDate DATETIME2(7)= GETUTCDATE();
            INSERT INTO dbo.Files
            ([Url], 
             [FileTypeId], 
             [CreatedBy], 
             [DateCreated]
            )
                   SELECT [Url], 
                          [FileTypeId], 
                          @CreatedBy, 
                          @NewDate
                   FROM @EventFiles
                   WHERE [Url] NOT IN
                   (
                       SELECT [URL]
                       FROM dbo.Files
                   );
			--- DELETE FROM ENTITY-IMAGES BRIDGE ---
			DECLARE @EntityTypeId INT = 3
			DELETE FROM dbo.EntityImages
			WHERE EntityId = @EventId
			AND EntityTypeId = @EntityTypeId 
            --- INSERT INTO ENTITY-IMAGES BRIDGE ---
            INSERT INTO dbo.EntityImages
            ([EntityId], 
             [FileId], 
             [EntityTypeId]
            )
                   SELECT @EventId, 
                          F.[Id], 
                          (SELECT [Id] FROM dbo.EntityTypes WHERE [Name] = 'Events')
                   FROM dbo.Files AS F
                   WHERE [Url] IN
                   (
                       SELECT [Url]
                       FROM @EventFiles
                   );
            COMMIT TRANSACTION @Tran;
        END TRY
        BEGIN CATCH
            IF XACT_STATE() = -1
                BEGIN
                    PRINT 'The transaction is in an uncommittable state.' + ' Rolling back transaction.';
                    ROLLBACK TRANSACTION @Tran;
            END;
            IF XACT_STATE() = 1
                BEGIN
                    PRINT 'The transaction is committable.' + ' Committing transaction.';
                    COMMIT TRANSACTION @Tran;
            END;
            THROW;
        END CATCH;
        SET XACT_ABORT OFF;
    END;