SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Session]
    @SessionId INT
AS
BEGIN
    SELECT * FROM Sessions
    WHERE SessionId = @SessionId
END;
GO