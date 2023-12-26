SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Product]
    @ProductId INT
AS
BEGIN
    SELECT * FROM Products WHERE ProductId = @ProductId;
    SELECT * FROM Product_Descriptions WHERE ProductId = @ProductId;
    SELECT * FROM Product_Categories WHERE ProductId = @ProductId;
    SELECT * FROM Product_Images WHERE ProductId = @ProductId;
    SELECT * FROM Product_Specs_Lookup WHERE ProductId = @ProductId;
    SELECT * FROM Product_Specs_Xml WHERE ProductId = @ProductId;
END;
GO
