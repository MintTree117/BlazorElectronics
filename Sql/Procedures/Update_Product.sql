SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Product]
    @ProductId INT,
    @VendorId INT,
    @Title NVARCHAR(64),
    @Thumbnail NVARCHAR(64),
    @ReleaseDate DATETIME,
    @IsFeatured BIT,
    @Price DECIMAL(18,2),
    @SalePrice DECIMAL(18,2) NULL,
    @Description NVARCHAR(1000),
    @TVP_CategoryIds TVP_CategoryIds READONLY,
    @TVP_Specs TVP_Specs READONLY,
    @TVP_Images TVP_Images READONLY,
    @XmlSpecs XML
AS
BEGIN
    -- Update base product
    UPDATE Products
    SET 
        VendorId = @VendorId,
        Title = @Title,
        Thumbnail = @Thumbnail,
        ReleaseDate = @ReleaseDate,
        IsFeatured = @IsFeatured,
        Price = @Price,
        SalePrice = @SalePrice
    WHERE ProductId = @ProductId;

    -- Update the product description
    UPDATE Product_Descriptions
    SET [Description] = @Description
    WHERE ProductId = @ProductId;

    -- Update xml specs
    UPDATE Product_Specs_Xml
    SET XmlSpecs = @XmlSpecs
    WHERE ProductId = @ProductId;

    -- Delete non-existent categories
    DELETE FROM Product_Categories
    WHERE ProductId = @ProductId AND CategoryId NOT IN (SELECT CategoryId FROM @TVP_CategoryIds);

    -- Add new categories
    INSERT INTO Product_Categories (ProductId, CategoryId)
    SELECT @ProductId, CategoryId
    FROM @TVP_CategoryIds
    WHERE CategoryId NOT IN (SELECT CategoryId FROM Product_Categories WHERE ProductId = @ProductId);

    -- Delete non-existent specs
    DELETE FROM Product_Specs_Lookup
    WHERE ProductId = @ProductId AND SpecId NOT IN (SELECT SpecId FROM @TVP_Specs);

    -- Add new specs
    INSERT INTO Product_Specs_Lookup (ProductId, SpecId, SpecValueId)
    SELECT @ProductId, SpecId, SpecValueId
    FROM @TVP_Specs
    WHERE SpecId NOT IN (SELECT SpecId FROM Product_Specs_Lookup WHERE ProductId = @ProductId);

    -- Delete non-existent images
    DELETE FROM Product_Images
    WHERE ProductId = @ProductId;

    -- Re-Insert all images
    INSERT INTO Product_Images (ProductId, ImageUrl)
    SELECT @ProductId, ImageUrl
    FROM @TVP_Images;
END;
GO
