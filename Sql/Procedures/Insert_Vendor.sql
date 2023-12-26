SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Vendor]
    @VendorName NVARCHAR(64),
    @VendorUrl NVARCHAR(128),
    @TVP_CategoryIds TVP_CategoryIds READONLY,
    @IsGlobal BIT
AS
BEGIN
    INSERT INTO Vendors (VendorName, VendorUrl, IsGlobal)
    VALUES (@VendorName, @VendorUrl, @IsGlobal);

    DECLARE @VendorId INT;
    SET @VendorId = SCOPE_IDENTITY();
    
    IF EXISTS (SELECT 1 FROM @TVP_CategoryIds)
    BEGIN
        INSERT INTO Vendor_Categories
        SELECT @VendorId, CategoryId
        FROM @TVP_CategoryIds;
    END;

    EXEC Update_Cache "Vendors";

    SELECT @VendorId;
END;
GO
