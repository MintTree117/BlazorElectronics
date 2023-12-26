SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertOrUpdate_CartItem]
    @UserId INT,
    @ProductId INT,
    @Quantity INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM Cart_Items
        WHERE UserId = @UserId
        AND ProductId = @ProductId
    )
    BEGIN
        INSERT INTO Cart_Items (UserId, ProductId, Quantity)
        VALUES (@UserId, @ProductId, @Quantity);
    END
    ELSE
    BEGIN
        UPDATE Cart_Items
        SET Quantity = @Quantity
        WHERE UserId = @UserId
        AND ProductId = @ProductId;
    END;
END;
GO
