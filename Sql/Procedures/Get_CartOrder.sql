SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Get_CartOrder]
    @UserId INT
AS
BEGIN
    SELECT p.ProductId, p.Price, p.SalePrice, c.Quantity FROM Products p
    INNER JOIN Cart_Items c ON p.ProductId = c.ProductId
    WHERE UserId = @UserId;
    
    SELECT * FROM Promos p
    INNER JOIN Cart_Promos c ON p.PromoId = c.PromoId
    WHERE c.UserId = @UserId;
END;
GO
