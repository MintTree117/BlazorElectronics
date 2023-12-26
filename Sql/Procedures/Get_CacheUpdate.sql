SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Get_CacheUpdate]
    @CacheName NVARCHAR(64)
AS
BEGIN
    SELECT LastUpdate FROM Cache_Updates
    WHERE CacheName = @CacheName;
END;
GO
