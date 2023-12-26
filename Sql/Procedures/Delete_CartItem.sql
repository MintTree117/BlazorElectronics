SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_CartItem]
    @UserId INT,
    @ProductId INT
AS
BEGIN
    DELETE FROM Cart_Items
    WHERE UserId = @UserId
    AND ProductId = @ProductId;

    SELECT ProductId, ItemQuantity FROM Cart_Items
    WHERE UserId = @UserId;
END;
GO
