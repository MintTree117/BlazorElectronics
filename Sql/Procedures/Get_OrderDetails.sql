SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_OrderDetails]
    @OrderId INT
AS
BEGIN
    SELECT * FROM Orders
    WHERE OrderId = @OrderId;

    SELECT * FROM Order_Items
    WHERE OrderId = @OrderId;
END;
GO
