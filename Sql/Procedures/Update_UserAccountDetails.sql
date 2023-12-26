SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_UserAccountDetails]
    @UserId INT,
    @Username NVARCHAR(64),
    @Email NVARCHAR(128),
    @Phone NVARCHAR(10)
AS
BEGIN
    UPDATE User_Accounts
    SET Username = @Username, Email = @Email, Phone = @Phone
    WHERE UserId = @UserId;

    SELECT Username, Email, Phone FROM User_Accounts
    WHERE UserId = @UserId;
END;
GO
