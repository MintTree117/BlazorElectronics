SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Review]
    @ProductId INT,
    @UserId INT,
    @Rating FLOAT,
    @Review NVARCHAR(500)
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM Reviews 
        WHERE ProductId = @ProductId AND UserId = @UserId
    )
    BEGIN
        -- Insert the review only if it does not exist
        INSERT INTO Reviews (ProductId, UserId, Rating, Review, [Date])
        VALUES (@ProductId, @UserId, @Rating, @Review, GETDATE());

        SELECT SCOPE_IDENTITY();
    END
END;
GO
