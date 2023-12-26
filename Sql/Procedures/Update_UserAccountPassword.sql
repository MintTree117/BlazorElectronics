SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_UserAccountPassword]
    @UserId INT,
    @Hash VARBINARY(64),
    @Salt VARBINARY(16)
AS
BEGIN
    UPDATE User_Accounts
    SET PasswordHash = @Hash, PasswordSalt = @Salt
    WHERE UserId = @UserId
END;
GO
