SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_ProductReviews]
    @ProductId INT,
    @Rows INT,
    @Offset INT,
    @ReviewSort INT
AS
BEGIN
    SELECT r.*, u.Username, TotalCount = Count(*) OVER() 
    FROM Reviews r
    INNER JOIN User_Accounts u ON r.UserId = u.UserId
    WHERE r.ProductId = @ProductId
    ORDER BY 
        CASE @ReviewSort 
            WHEN 1 THEN r.Date -- Assuming you have a 'Date' column for the date
            WHEN 2 THEN r.Rating
            WHEN 3 THEN -r.Rating -- Negative rating for inverse ordering
            ELSE r.UserId -- Default ordering if @ReviewSort is not 1, 2, or 3
        END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @Rows ROWS ONLY;
END;
GO
