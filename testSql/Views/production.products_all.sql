SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
create view [production].[products_all]
as
select 
	p.product_id,
	b.brand_name,
	c.category_name,
	p.model_year,
	p.list_price
from production.products p
inner join production.brands b
on b.brand_id = p.brand_id
inner join production.categories c
on c.category_id = p.category_id
GO
