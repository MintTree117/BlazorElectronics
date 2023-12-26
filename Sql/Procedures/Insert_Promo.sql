SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Promo]
    @PromoCode NVARCHAR(64),
    @PromoDiscount FLOAT
AS
BEGIN
    INSERT INTO Promos (PromoCode, PromoDiscount)
    VALUES (@PromoCode, @PromoDiscount);

    SELECT SCOPE_IDENTITY();
END;
GO
