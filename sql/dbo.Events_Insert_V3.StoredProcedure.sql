USE [C84_InternsLA]
GO
/****** Object:  StoredProcedure [dbo].[Events_Insert_V3]    Script Date: 4/9/2020 8:08:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Events_Insert_V3] @EventTypeId      INT, 
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
                                    @LocationLineTwo  NVARCHAR(255) = null, 
                                    @LocationCity     NVARCHAR(225), 
                                    @LocationZip      NVARCHAR(50), 
                                    @LocationStateId  INT, 
                                    @LocationLat      FLOAT, 
                                    @LocationLong     FLOAT, 
                                    @DateStart        DATETIME2(7), 
                                    @DateEnd          DATETIME2(7), 
                                    @CreatedBy        INT, 
                                    @ModifiedBy       INT, 
                                    @EventId          INT OUTPUT
AS

/*

DECLARE 
	@EventTypeId INT= 5, 
	@EntityTypeId INT = 3,
	@Name NVARCHAR(255)= 'Alberto Surprise Party', 
	@Summary NVARCHAR(255)= 'Birthday party', 
	@ShortDescription NVARCHAR(4000)= 'Party', 
	@EventStatusId INT= 3, 
	@ImageUrl NVARCHAR(400)= 'www.pic.com', 
	@ExternalSiteUrl NVARCHAR(400)= 'www.pic.com', 
	@IsFree INT= 1, 
	@VenueId INT= 0, 
	@VenueName NVARCHAR(255)= 'Sabio HQ', 
	@VenueDescription NVARCHAR(4000)= 'Code Camp', 
	@VenueUrl NVARCHAR(255)= 'www.sabio.la', 
	@LocationId INT= 0, 
	@LocationTypeId INT= 2, 
	@LocationLineOne NVARCHAR(255)= '400 Corporate Pointe', 
	@LocationLineTwo NVARCHAR(255), 
	@LocationCity NVARCHAR(225)= 'Culver City', 
	@LocationZip NVARCHAR(50)= '90280', 
	@LocationStateId INT= 9, 
	@LocationLat FLOAT= 33.988440, 
	@LocationLong FLOAT= -118.384580, 
	@DateStart DATETIME2(7)= GETUTCDATE(), 
	@DateEnd DATETIME2(7)= GETUTCDATE(), 
	@CreatedBy INT= 8, 
	@ModifiedBy INT= 8, 
	@EventId INT;

DECLARE @EventFiles AS FILETABLETYPE;
INSERT INTO @EventFiles
([Url], 
 [FileTypeId], 
 [CreatedBy]
)VALUES ('www.a345baba.com', 2, @CreatedBy), ('www.ki7805baba.com', 2, @CreatedBy)
       

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
FROM DBO.[Events];
EXECUTE [dbo].[Events_Insert_V3] 
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
        @ModifiedBy, 
        @EventId OUTPUT;
SELECT *, 
(
    SELECT EI.FileId, 
           F.Url
    FROM dbo.EntityImages EI
         JOIN dbo.Files F ON EI.FileId = F.Id
    WHERE EI.EntityId = @EventId
          AND EI.EntityTypeId = @EntityTypeId FOR JSON PATH
) AS EventFiles
FROM DBO.[Events] AS E
     JOIN DBO.Venues AS V ON E.VenueId = V.Id
     JOIN DBO.Locations AS L ON V.LocationId = L.Id;
*/

    BEGIN
        SET XACT_ABORT ON;
        DECLARE @Tran NVARCHAR(50)= 'Event_Insert_Tx';
        BEGIN TRY
            BEGIN TRANSACTION @Tran;
            DECLARE @EntityTypeId INT= 3;

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

            --- INSERT INTO EVENTS TABLE ---
            EXEC dbo.Events_Insert_V2 
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
                 @CreatedBy, 
                 @EventId OUTPUT;

            --- INSERT EVENT FILES INTO FILES TABLE ---
            EXEC dbo.Files_Insert_V3 
                 @EventFiles;

            --- INSERT INTO ENTITY-IMAGES BRIDGE ---
            INSERT INTO dbo.EntityImages
            ([EntityId], 
             [FileId], 
             [EntityTypeId]
            )
                   SELECT @EventId, 
                          F.[Id], 
                          @EntityTypeId
                   FROM dbo.Files AS F
                   WHERE F.[Url] IN
                   (
                       SELECT E.[Url]
                       FROM @EventFiles AS E
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
GO
