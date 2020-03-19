CREATE TABLE [production].[products]
(
[product_id] [int] NOT NULL IDENTITY(1, 1),
[product_name] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[brand_id] [int] NOT NULL,
[category_id] [int] NOT NULL,
[model_year] [smallint] NOT NULL,
[list_price] [decimal] (10, 2) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [production].[products] ADD CONSTRAINT [PK__products__47027DF5E59BA3A1] PRIMARY KEY CLUSTERED  ([product_id]) ON [PRIMARY]
GO
ALTER TABLE [production].[products] ADD CONSTRAINT [FK__products__brand___29572725] FOREIGN KEY ([brand_id]) REFERENCES [production].[brands] ([brand_id]) ON DELETE CASCADE ON UPDATE CASCADE
GO
ALTER TABLE [production].[products] ADD CONSTRAINT [FK__products__catego__286302EC] FOREIGN KEY ([category_id]) REFERENCES [production].[categories] ([category_id]) ON DELETE CASCADE ON UPDATE CASCADE
GO
