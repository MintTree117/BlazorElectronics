SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Get_FeaturedDeals]
    @Offset INT,
    @Rows INT
AS
BEGIN
    SELECT ProductId, Title, Thumbnail, Price, SalePrice, Rating, NumberReviews 
    FROM Products
    WHERE IsFeatured = 1
    AND SalePrice IS NOT NULL
    ORDER BY NumberReviews DESC
    OFFSET @Offset ROWS
    FETCH NEXT @Rows ROWS ONLY;
END;
GO
