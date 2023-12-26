SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Get_EmailByVerificationToken]
    @VerificationToken NVARCHAR(128)
AS
BEGIN
    SELECT UserEmail FROM User_Verifications
    WHERE Token = @VerificationToken;
END;
GO
