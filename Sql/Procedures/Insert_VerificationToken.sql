SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_VerificationToken]
    @UserId INT,
    @Email NVARCHAR(64),
    @Token NVARCHAR(128)
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM User_Verifications
        WHERE UserId = @UserId
        AND Token = @Token)
    BEGIN
        INSERT INTO User_Verifications (UserId, UserEmail, Token)
        VALUES (@UserId, @Email, @Token);
    END;
END;
GO
