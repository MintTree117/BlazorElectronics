SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_CartItem]
    @UserId INT,
    @ProductId INT,
    @Quantity INT
AS
BEGIN
    UPDATE Cart_Items
    SET Quantity = @Quantity
    WHERE UserId = @UserId
    AND ProductId = @ProductId;

    SELECT ProductId, Quantity 
    FROM Cart_Items
    WHERE UserId = @UserId;
END;
GO
