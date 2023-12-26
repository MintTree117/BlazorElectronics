SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Get_Cart]
    @UserId INT
AS
BEGIN
    SELECT c.*, p.Title, p.Thumbnail, p.Price, p.SalePrice FROM Cart_Items c
    INNER JOIN Products p ON p.ProductId = c.ProductId
    WHERE UserId = @UserId;

    SELECT * FROM Cart_Promos
    WHERE UserId = @UserId;
END;
GO
