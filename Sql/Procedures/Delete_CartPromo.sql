SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Delete_CartPromo]
    @UserId INT,
    @PromoId INT
AS
BEGIN
    DELETE FROM Cart_Promos
    WHERE UserId = @UserId
    AND PromoId = @PromoId;
END;
GO
