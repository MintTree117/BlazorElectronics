SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_VendorEdit]
    @VendorId INT
AS
BEGIN
    SELECT * FROM Vendors WHERE VendorId = @VendorId;
    SELECT * FROM Vendor_Categories WHERE VendorId = @VendorId;
END;
GO
