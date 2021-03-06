USE [C84_InternsLA]
GO
/****** Object:  StoredProcedure [dbo].[Admin_Dashboard_SelectAll_RecentMetrics_V2]    Script Date: 4/9/2020 8:08:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Admin_Dashboard_SelectAll_RecentMetrics_V2]
AS

/*

EXECUTE [dbo].[Admin_Dashboard_SelectAll_RecentMetrics_V2]

*/

    BEGIN
        DECLARE @lastYear DATETIME2(7)= DATEADD(year, -1, GETUTCDATE());
        SELECT
        (
            SELECT O.[Id], 
                   O.[Name], 
                   O.[Logo], 
                   O.[SiteUrl], 
                   O.[DateCreated], 
                   L.[City], 
                   S.[Name] AS StateName, 
                   S.[StateProvinceCode]
            FROM [dbo].[Organizations] AS O
                 JOIN [dbo].Locations AS L ON L.Id = O.LocationId
                 JOIN [dbo].States AS S ON L.StateId = S.Id
            WHERE O.DateCreated >= @lastYear
            ORDER BY O.DateCreated DESC FOR JSON PATH
        ) AS RecentOrganizations, 
        (
            SELECT UP.UserId, 
                   UP.FirstName, 
                   UP.LastName, 
                   UP.AvatarUrl, 
                   UP.DateCreated
            FROM [dbo].UserProfiles AS UP
            WHERE UP.DateCreated >= @lastYear
            ORDER BY [UP].DateCreated FOR JSON PATH
        ) AS RecentUsers, 
        (
            SELECT O.[Id], 
                   O.[Title], 
                   JT.[Type] AS JobType, 
                   L.City, 
                   O.DateCreated
            FROM [dbo].[Openings] AS O
                 JOIN [dbo].JobType AS JT ON JT.Id = O.JobTypeId
                 LEFT JOIN [dbo].Locations AS L ON L.Id = O.LocationId
                                                   AND IsActive = 1
            WHERE O.DateCreated >= @lastYear
            ORDER BY O.DateCreated DESC FOR JSON PATH
        ) AS RecentJobs, 
        (
            SELECT E.[Id], 
                   E.[Name] AS EventName, 
                   E.DateStart, 
                   V.[Name] AS VenueName, 
                   L.City, 
                   S.[Name] AS StateName, 
                   S.[StateProvinceCode]
            FROM [dbo].[Events] AS E
                 JOIN [dbo].Venues AS V ON V.Id = E.VenueId
                 JOIN dbo.Locations AS L ON L.Id = V.Id
                 JOIN [dbo].States AS S ON L.StateId = S.Id
            WHERE E.DateCreated >= @lastYear
            ORDER BY E.DateStart DESC FOR JSON PATH
        ) AS RecentEvents;
    END;
GO
