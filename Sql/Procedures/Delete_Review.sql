SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Delete_Review]
    @ReviewId INT
AS
BEGIN
    DELETE FROM Reviews
    WHERE ReviewId = @ReviewId;
END;
GO