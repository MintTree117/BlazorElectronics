SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_VerificationToken]
    @Token NVARCHAR(128)
AS
BEGIN
    UPDATE User_Verifications
    SET IsUsed = 1
    WHERE Token = @Token;

    SELECT UserId FROM User_Verifications
    WHERE Token = @Token;
END;
GO
