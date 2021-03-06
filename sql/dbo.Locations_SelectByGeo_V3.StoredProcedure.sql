USE [C84_InternsLA]
GO
/****** Object:  StoredProcedure [dbo].[Locations_SelectByGeo_V3]    Script Date: 4/9/2020 8:08:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Locations_SelectByGeo_V3] @Latitude  FLOAT, 
                                             @Longitude FLOAT, 
                                             @Radius    INT, 
                                             @PageIndex INT, 
                                             @PageSize  INT

/*

DECLARE
    @_lat FLOAT = 32.19,
    @_lng FLOAT = 12.34,
    @_radius int = 100000, 
	@_pageIndex int = 0,
	@_pageSize int = 3

	SELECT * FROM dbo.Locations
EXECUTE [dbo].[Locations_SelectByGeo_V3] @_lat, @_lng, @_radius, @_pageIndex, @_pageSize


 
*/

AS
    BEGIN
        DECLARE @MetersPerMile FLOAT= 1609.344, @SRID INT= 4326;
        DECLARE @LocationStart GEOGRAPHY= GEOGRAPHY ::Point(@Latitude, @Longitude, @SRID);
        DECLARE @OffSet INT= @PageIndex * @PageSize;
        SELECT L.[Id], 
               LT.[Name] AS LocationTypeName, 
               LT.[Id] AS LocationTypeId, 
               L.[LineOne], 
               L.[LineTwo], 
               L.[City], 
               L.[Zip], 
               S.[Id] AS StateId, 
               S.[Name], 
               S.[StateProvinceCode], 
               S.[CountryRegionCode], 
               L.[Latitude], 
               L.[Longitude], 
               L.[DateCreated], 
               L.[DateModified], 
               UP.[FirstName], 
               UP.[LastName], 
               TotalCount = COUNT(1) OVER()
        FROM [dbo].[Locations] AS L
		 LEFT JOIN LocationTypes AS LT ON LocationTypeId = LT.Id
             LEFT JOIN UserProfiles AS UP ON L.CreatedBy = UP.UserId
             LEFT JOIN States AS S ON L.StateId = S.Id
        WHERE @LocationStart.STDistance(GeoPoint) / @MetersPerMile <= @Radius --miles
        ORDER BY L.Id
        OFFSET @OffSet ROWS FETCH NEXT @PageSize ROWS ONLY;
    END;
GO
