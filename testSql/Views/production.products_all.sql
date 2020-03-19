SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE VIEW [production].[products_all]
AS
SELECT p.product_id,
    b.brand_name,
    c.category_name,
    p.model_year,
    p.list_price
FROM production.products p
INNER JOIN production.brands b
    ON b.brand_id = p.brand_id
INNER JOIN production.categories c
    ON c.category_id = p.category_id
GO


