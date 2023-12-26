SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_SpecEdit]
    @SpecId INT
AS
BEGIN
    SELECT * FROM Specs_Lookup WHERE SpecId = @SpecId;
    SELECT * FROM Specs_Lookup_Categories WHERE SpecId = @SpecId;
    SELECT * FROM Specs_Lookup_Values WHERE SpecId = @SpecId;
END;
GO
