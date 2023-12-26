SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Feature]
    @FeatureId INT,
    @Name NVARCHAR(64),
    @Url NVARCHAR(128),
    @Image NVARCHAR(64)
AS
BEGIN
    UPDATE Features
    SET [Name] = @Name, [Url] = @Url, [Image] = @Image
    WHERE FeatureId = @FeatureId;

    EXEC Update_Cache "Features";
END;
GO
