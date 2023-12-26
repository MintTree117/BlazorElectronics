SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Spec]
    @SpecId INT,
    @SpecName NVARCHAR(64),
    @TVP_SpecValues TVP_SpecValues READONLY,
    @TVP_CategoryIds TVP_CategoryIds READONLY,
    @IsGlobal BIT,
    @IsAvoid BIT
AS
BEGIN
    IF @SpecName IS NOT NULL
    BEGIN
        UPDATE Specs_Lookup
        SET SpecName = @SpecName, IsGlobal = @IsGlobal, IsAvoid = @IsAvoid
        WHERE SpecId = @SpecId;
    END;

    DELETE FROM Specs_Lookup_Values
    WHERE SpecId = @SpecId;

    INSERT INTO Specs_Lookup_Values (SpecId, SpecValueId, SpecValue)
    SELECT @SpecId, SpecValueId, SpecValue
    FROM @TVP_SpecValues;

    IF EXISTS (SELECT 1 FROM @TVP_CategoryIds)
    BEGIN
        -- Remove entries that don't exist in the @PrimaryCategories parameter
        DELETE FROM Specs_Lookup_Categories
        WHERE SpecId = @SpecId
        AND PrimaryCategoryId NOT IN (SELECT PrimaryCategoryId FROM @TVP_CategoryIds);

        -- Insert new entries from @PrimaryCategories that don't exist in the table
        INSERT INTO Specs_Lookup_Categories (SpecId, PrimaryCategoryId)
        SELECT @SpecId, pc.CategoryId
        FROM @TVP_CategoryIds AS pc
        WHERE NOT EXISTS (
            SELECT 1
            FROM Specs_Lookup_Categories AS existing
            WHERE existing.SpecId = @SpecId
            AND existing.PrimaryCategoryId = pc.CategoryId
        );
    END;

    EXEC Update_Cache "SpecLookups";
END;
GO
