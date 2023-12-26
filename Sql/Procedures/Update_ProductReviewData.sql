SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_ProductReviewData]
AS
BEGIN
    UPDATE P
    SET P.Rating = ISNULL(PR.AvgRating, 0) -- or another default value for products with no reviews
    FROM Products P
    LEFT JOIN (
        SELECT ProductId, AVG(Rating) AS AvgRating
        FROM Reviews
        GROUP BY ProductId
    ) PR ON P.ProductId = PR.ProductId;

    UPDATE P
    SET P.NumberReviews = ISNULL(PR.ReviewCount, 0)
    FROM Products P
    LEFT JOIN (
        SELECT ProductId, COUNT(*) AS ReviewCount
        FROM Reviews
        GROUP BY ProductId
    ) PR ON P.ProductId = PR.ProductId;
END;
GO
