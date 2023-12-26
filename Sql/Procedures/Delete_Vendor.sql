SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Delete_Vendor]
    @VendorId INT
AS
BEGIN
    DELETE FROM Vendors 
    WHERE VendorId = @VendorId;

    EXEC Update_Cache "Vendors";
END;
GO
