SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_UserAccountExists]
    @Email NVARCHAR(254),
    @Username NVARCHAR(48)
AS
BEGIN
    SELECT 
        COALESCE(Email, NULL) AS Email,
        COALESCE(Username, NULL) AS Username
    FROM 
        User_Accounts
    WHERE 
        Email = @Email OR Username = @Username;
END;
GO
