SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Review]
    @ReviewId INT,
    @Rating FLOAT,
    @Review NVARCHAR(500)
AS
BEGIN
    UPDATE Reviews
    SET Rating = @Rating, Review = @Review, [Date] = GETDATE()
    WHERE ReviewId = @ReviewId;
END;
GO
