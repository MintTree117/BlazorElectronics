SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_UserAccountByUsername]
    @Username NVARCHAR(48)
AS
BEGIN
    SELECT * FROM User_Accounts
    WHERE Username = @Username
END;
GO
