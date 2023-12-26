SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Product]
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
    INSERT INTO Products (
        VendorId,
        Title,
        Thumbnail,
        ReleaseDate,
        IsFeatured,
        Price,
        SalePrice
    )
    VALUES (
        @VendorId,
        @Title,
        @Thumbnail,
        @ReleaseDate,
        @IsFeatured,
        @Price,
        @SalePrice
    );

    DECLARE @ProductId INT;
    SET @ProductId = SCOPE_IDENTITY();

    INSERT INTO Product_Categories (ProductId, CategoryId)
    SELECT @ProductId, CategoryId
    FROM @TVP_CategoryIds;

    INSERT INTO Product_Specs_Lookup (ProductId, SpecId, SpecValueId)
    SELECT @ProductId, SpecId, SpecValueId
    FROM @TVP_Specs;

    INSERT INTO Product_Specs_Xml (ProductId, XmlSpecs)
    VALUES (@ProductId, @XmlSpecs);

    INSERT INTO Product_Images (ProductId, ImageUrl)
    SELECT @ProductId, ImageUrl
    FROM @TVP_Images;

    INSERT INTO Product_Descriptions (ProductId, [Description])
    VALUES (@ProductId, @Description);

    SELECT @ProductId;
END;
GO
