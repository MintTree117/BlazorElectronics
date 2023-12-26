SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_ProductSuggestions]
    @SearchText NVARCHAR(64)
AS
BEGIN
    SELECT TOP 20 p.Title
    FROM Products p
    INNER JOIN Product_Descriptions pd ON p.ProductId = pd.ProductId
    WHERE p.Title LIKE '%' + @SearchText + '%'
    OR pd.Description LIKE '%' + @SearchText + '%'
    ORDER BY 
        CASE 
            WHEN p.Title LIKE '%' + @SearchText + '%' THEN 1
            WHEN pd.Description LIKE '%' + @SearchText + '%' THEN 2
            ELSE 3 
        END,
        p.Title; -- Or any other field you deem relevant for secondary sorting
END;
GO
