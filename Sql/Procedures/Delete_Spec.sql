SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Delete_Spec]
    @SpecId INT
AS
BEGIN
    DELETE FROM Specs_Lookup
    WHERE SpecId = @SpecId;

    EXEC Update_Cache "SpecLookups";
END;
GO
