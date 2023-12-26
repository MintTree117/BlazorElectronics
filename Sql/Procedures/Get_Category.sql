SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Get_Category]
    @CategoryId INT
AS
BEGIN
    SELECT * FROM Categories
    WHERE CategoryId = @CategoryId;
END;
GO
