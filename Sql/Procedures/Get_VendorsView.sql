SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_VendorsView]
AS
BEGIN
    SELECT * FROM Vendors;
END;
GO