SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Cart]
    @UserId INT,
    @TVP_CartItems TVP_CartItems READONLY
AS
BEGIN
    MERGE INTO Cart_Items AS target
    USING (SELECT ProductId, Quantity FROM @TVP_CartItems) AS source (ProductId, Quantity)
    ON (target.UserId = @UserId AND target.ProductId = source.ProductId)
    WHEN MATCHED THEN 
        UPDATE SET target.Quantity = source.Quantity
    WHEN NOT MATCHED THEN   
        INSERT (UserId, ProductId, Quantity)
        VALUES (@UserId, source.ProductId, source.Quantity);

    SELECT p.ProductId, p.Title, p.Thumbnail, p.Price, p.SalePrice, c.Quantity FROM Products p
    INNER JOIN Cart_Items c ON p.ProductId = c.ProductId
    WHERE UserId = @UserId;
    
    SELECT * FROM Promos p
    INNER JOIN Cart_Promos c ON p.PromoId = c.PromoId
    WHERE c.UserId = @UserId;
END;
GO
