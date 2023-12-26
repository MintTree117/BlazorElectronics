SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_ProductKeys]
    @ProductId INT,
    @Keys TVP_ProductKeys READONLY
AS
BEGIN
    INSERT INTO Product_Keys
    (ProductId, [Key])
    SELECT @ProductId, ProductKey FROM @Keys;
END;
GO
