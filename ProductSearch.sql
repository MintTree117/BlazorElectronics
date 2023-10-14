WITH FilteredProducts AS (
    SELECT p.ProductId AS ProductIdAlias, p.ProductName
    FROM Products p
    LEFT JOIN ProductCategories pc ON p.ProductId = pc.ProductId
    LEFT JOIN ProductDescriptions pd ON p.ProductId = pd.ProductId
    WHERE 1=1
      AND pc.CategoryId = 1
      AND (p.ProductName LIKE '%cpu%' OR pd.DescriptionBody LIKE '%cpu%')
)
, ProductVariantsCTE AS (
    SELECT pv.ProductId AS ProductIdAlias, pv.VariantId, pv.VariantName, pv.VariantPriceMain
    FROM ProductVariants pv
)
SELECT f.ProductIdAlias AS ProductId, f.ProductName,
(
    SELECT pv.VariantId, pv.VariantName, pv.VariantPriceMain
    FROM ProductVariantsCTE pv
    WHERE f.ProductIdAlias = pv.ProductIdAlias
    FOR XML PATH('Variant'), ROOT('Variants'), TYPE
) AS Variant
FROM FilteredProducts f
WHERE EXISTS (
    SELECT *
    FROM ProductVariantsCTE pv
    WHERE f.ProductIdAlias = pv.ProductIdAlias
    AND pv.VariantPriceMain >= 1
)
ORDER BY f.ProductIdAlias
OFFSET 0 ROWS
FETCH NEXT 10 ROWS ONLY;
