SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_UserAccountDetails]
    @UserId INT
AS
BEGIN
    SELECT Username, Email, Phone FROM User_Accounts
    WHERE UserId = @UserId;
END;
GO
