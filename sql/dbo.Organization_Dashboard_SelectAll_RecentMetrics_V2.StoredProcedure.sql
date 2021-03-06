USE [C84_InternsLA]
GO
/****** Object:  StoredProcedure [dbo].[Organization_Dashboard_SelectAll_RecentMetrics_V2]    Script Date: 4/9/2020 8:08:37 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Organization_Dashboard_SelectAll_RecentMetrics_V2] @OrganizationId INT
AS

/*
DECLARE @OrganizationId INT= 4;
EXECUTE [dbo].[Organization_Dashboard_SelectAll_RecentMetrics]
        @OrganizationId;
*/

    BEGIN
        DECLARE @lastYear DATETIME2(7)= DATEADD(year, -1, GETUTCDATE());
        SELECT
        (
            SELECT OM.UserId, 
                         UP.FirstName, 
                         UP.LastName, 
                         UP.AvatarUrl, 
                         OM.DateCreated
            FROM [dbo].[OrganizationMembers] AS OM
                 JOIN [dbo].UserProfiles AS UP ON UP.UserId = OM.UserId
            WHERE OM.OrganizationId = @OrganizationId AND OM.DateCreated >= @lastYear
            ORDER BY OM.DateCreated DESC FOR JSON PATH
        ) AS RecentMembers, 
        (
            SELECT [OF].[FollowerId] AS UserId, 
                         UP.FirstName, 
                         UP.LastName, 
                         UP.AvatarUrl, 
                         [OF].DateFollowed
            FROM [dbo].OrganizationFollowers AS [OF]
                 JOIN [dbo].UserProfiles AS UP ON UP.UserId = [OF].FollowerId
            WHERE OrgId = @OrganizationId AND [OF].DateFollowed >= @lastYear
            ORDER BY [OF].DateFollowed DESC FOR JSON PATH
        ) AS RecentFollowers, 
        (
            SELECT TOP 6 O.[Id], 
                         O.[Title], 
                         JT.[Type] AS JobType, 
                         L.City, 
                         O.DateCreated
            FROM [dbo].[Openings] AS O
                 JOIN [dbo].JobType AS JT ON JT.Id = O.JobTypeId
                 LEFT JOIN [dbo].Locations AS L ON L.Id = O.LocationId
            WHERE OrganizationId = @OrganizationId
                  AND IsActive = 1
            ORDER BY O.DateCreated DESC FOR JSON PATH
        ) AS RecentJobs;
    END;
GO
