SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
create procedure [production].[Brands_GetAll]
AS
BEGIN
	Set NOCOUNT ON;

	SELECT
		brand_id,
		brand_name
	FROM production.brands
	ORDER by
		brand_name;
END

GO
