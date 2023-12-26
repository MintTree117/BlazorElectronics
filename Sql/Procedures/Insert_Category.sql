SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Category]
    @ParentCategoryId INT NULL,
    @Tier INT,
    @Name NVARCHAR(64),
    @ApiUrl NVARCHAR(64),
    @ImageUrl NVARCHAR(64)
AS
BEGIN
    INSERT INTO Categories (ParentCategoryId, Tier, [Name], ApiUrl, ImageUrl)
    VALUES (@ParentCategoryId, @Tier, @Name, @ApiUrl, @ImageUrl);
    
    EXEC Update_Cache "Categories";

    SELECT SCOPE_IDENTITY();
END;
GO
