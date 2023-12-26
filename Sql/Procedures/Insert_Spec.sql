SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Spec]
    @SpecName NVARCHAR(64),
    @TVP_SpecValues TVP_SpecValues READONLY,
    @TVP_CategoryIds TVP_CategoryIds READONLY,
    @IsGlobal BIT,
    @IsAvoid BIT
AS
BEGIN
    INSERT INTO Specs_Lookup (SpecName, IsGlobal, IsAvoid)
    VALUES (@SpecName, @IsGlobal, @IsAvoid);

    DECLARE @SpecId INT;
    SET @SpecId = SCOPE_IDENTITY();

    IF EXISTS (SELECT 1 FROM @TVP_SpecValues)
    BEGIN
        INSERT INTO Specs_Lookup_Values
        SELECT @SpecId, SpecValueId, SpecValue
        FROM @TVP_SpecValues;
    END;
    
    IF EXISTS (SELECT 1 FROM @TVP_CategoryIds)
    BEGIN
        INSERT INTO Specs_Lookup_Categories
        SELECT @SpecId, CategoryId
        FROM @TVP_CategoryIds;
    END;

    EXEC Update_Cache "SpecLookups";

    SELECT @SpecId;
END;
GO
