SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_Promo]
    @PromoId INT,
    @PromoCode NVARCHAR(64),
    @PromoDiscount FLOAT
AS
BEGIN
    UPDATE Promos
    SET PromoCode = @PromoCode, PromoDiscount = @PromoDiscount
    WHERE PromoId = @PromoId;
END;
GO
