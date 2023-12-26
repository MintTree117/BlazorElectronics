SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Specs]
AS
BEGIN
    SELECT * FROM Specs_Lookup;
    SELECT * FROM Specs_Lookup_Categories;
    SELECT * FROM Specs_Lookup_Values;
END;
GO
