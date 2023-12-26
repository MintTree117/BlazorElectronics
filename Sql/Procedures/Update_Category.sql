SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Category]
    @CategoryId INT,
    @ParentCategoryId INT NULL,
    @Tier INT,
    @Name NVARCHAR(64),
    @ApiUrl NVARCHAR(64),
    @ImageUrl NVARCHAR(64)
AS
BEGIN
    UPDATE Categories
    SET ParentCategoryId = @ParentCategoryId, Tier = @Tier, [Name] = @Name, ApiUrl = @ApiUrl, ImageUrl = @ImageUrl
    WHERE CategoryId = @CategoryId;

    EXEC Update_Cache "Categories";
END;
GO
