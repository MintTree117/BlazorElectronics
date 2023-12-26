SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Vendor]
    @VendorId INT,
    @VendorName NVARCHAR(64),
    @VendorUrl NVARCHAR(128),
    @TVP_CategoryIds TVP_CategoryIds READONLY,
    @IsGlobal BIT
AS
BEGIN
    UPDATE Vendors
    SET VendorName = @VendorName, VendorUrl = @VendorUrl, IsGlobal = @IsGlobal
    WHERE VendorId = @VendorId;

    IF EXISTS (SELECT 1 FROM @TVP_CategoryIds)
    BEGIN
        -- Remove entries that don't exist in the @PrimaryCategories parameter
        DELETE FROM Vendor_Categories
        WHERE VendorId = @VendorId
        AND PrimaryCategoryId NOT IN (SELECT PrimaryCategoryId FROM @TVP_CategoryIds);

        -- Insert new entries from @PrimaryCategories that don't exist in the table
        INSERT INTO Vendor_Categories (VendorId, PrimaryCategoryId)
        SELECT @VendorId, pc.CategoryId
        FROM @TVP_CategoryIds AS pc
        WHERE NOT EXISTS (
            SELECT 1
            FROM Vendor_Categories AS existing
            WHERE existing.VendorId = @VendorId
            AND existing.PrimaryCategoryId = pc.CategoryId
        );
    END;

    EXEC Update_Cache "Vendors";
END;
GO
