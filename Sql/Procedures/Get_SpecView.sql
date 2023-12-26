SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_SpecView]
AS
BEGIN
    SELECT * FROM Specs_Lookup;
    SELECT * FROM Specs_Lookup_Values;
    SELECT * FROM Specs_Lookup_Categories;
END;
GO
