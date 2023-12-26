SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Feature]
    @Name NVARCHAR(64),
    @Url NVARCHAR(128),
    @Image NVARCHAR(64)
AS
BEGIN
    INSERT INTO Features ([Name], [Url], [Image])
    VALUES (@Name, @Url, @Image);

    EXEC Update_Cache "Features";

    SELECT SCOPE_IDENTITY();
END;
GO
