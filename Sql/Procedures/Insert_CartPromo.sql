SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_CartPromo]
    @UserId INT,
    @PromoCode NVARCHAR(64)
AS
BEGIN
    DECLARE @PromoId INT;

    SELECT @PromoId = PromoId FROM Promos
    WHERE PromoCode = @PromoCode;

    IF @PromoId IS NOT NULL
    AND NOT EXISTS (
        SELECT 1 FROM Cart_Promos
        WHERE UserId = @UserId
        AND PromoId = @PromoId
    )
    BEGIN
        INSERT INTO Cart_Promos (UserId, PromoId)
        VALUES (@UserId, @PromoId);
    END;

    SELECT * FROM Cart_Promos
    WHERE PromoId = @PromoId;
END;
GO
