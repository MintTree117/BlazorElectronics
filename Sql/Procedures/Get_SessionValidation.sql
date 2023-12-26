SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_SessionValidation]
    @SessionId INT
AS
BEGIN
    SELECT s.UserId, s.TokenHash, s.TokenSalt, s.IsActive, s.LastActivityDate, u.IsActive, u.IsAdmin
    FROM Sessions s
    INNER JOIN User_Accounts u
    ON s.UserId = u.UserId
    WHERE SessionId = @SessionId;
END;
GO
