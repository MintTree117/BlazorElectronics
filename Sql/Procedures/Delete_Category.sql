SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_Category]
    @CategoryId INT
AS
BEGIN
    DELETE FROM Categories
    WHERE CategoryId = @CategoryId;

    EXEC Update_Cache "Categories";
END;
GO
