SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Order]
    @UserId INT,
    @OrderDate DATETIME,
    @TotalPrice DECIMAL,
    @TVP_OrderItems TVP_OrderItems READONLY,
    @TVP_OrderPromos TVP_OrderPromos READONLY
AS
BEGIN
    DECLARE @OrderId INT;

    INSERT INTO Orders (UserId, OrderDate, TotalPrice)
    VALUES (@UserId, @OrderDate, @TotalPrice);

    SET @OrderId = SCOPE_IDENTITY();

    INSERT INTO Order_Items (OrderId, ProductId, Quantity, TotalPrice)
    SELECT @OrderId, ProductId, Quantity, TotalPrice
    FROM @TVP_OrderItems;

    INSERT INTO Order_Promos (OrderId, PromoCode, Discount)
    SELECT @OrderId, PromoCode, Discount
    FROM @TVP_OrderPromos;
END;
GO
