SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Cache]
    @CacheName NVARCHAR(64)
AS
BEGIN
    UPDATE Cache_Updates
    SET LastUpdate = GETDATE()
    WHERE CacheName = @CacheName;
END;
GO
