SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

CREATE PROCEDURE [production].[Brands_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT brand_id,
        brand_name
    FROM production.brands
    ORDER BY brand_name;
END
GO


