SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_CategoriesBulk]
    @TVP_Categories TVP_Categories READONLY
AS
BEGIN
    INSERT INTO Categories
    (ParentCategoryId, Tier, [Name], ApiUrl, ImageUrl)
    SELECT ParentCategoryId, Tier, [Name], ApiUrl, ImageUrl 
    FROM @TVP_Categories;
END;
GO
