SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Orders]
    @UserId INT
AS
BEGIN
    SELECT * FROM Orders
    WHERE UserId = @UserId;
END;
GO