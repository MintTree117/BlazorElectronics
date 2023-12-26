SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Delete_Feature]
    @FeatureId INT
AS
BEGIN
    DELETE FROM Features
    WHERE FeatureId = @FeatureId;

    EXEC Update_Cache "Features";
END;
GO
