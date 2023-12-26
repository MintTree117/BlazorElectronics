SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_UserAccountStatus]
    @UserId INT
AS
BEGIN
    UPDATE User_Accounts
    SET IsActive = 1
    WHERE UserId = @UserId;
END;
GO
