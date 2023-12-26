SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_UserValidation]
    @UserId INT
AS
BEGIN
    SELECT IsActive, IsAdmin FROM User_Accounts
    WHERE UserId = @UserId;
END;
GO
