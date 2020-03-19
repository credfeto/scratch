CREATE TABLE [sales].[order_items] (
    [order_id] [int] NOT NULL,
    [item_id] [int] NOT NULL,
    [product_id] [int] NOT NULL,
    [quantity] [int] NOT NULL,
    [list_price] [decimal](10, 2) NOT NULL,
    [discount] [decimal](4, 2) NOT NULL CONSTRAINT [DF__order_ite__disco__398D8EEE] DEFAULT((0))
    ) ON [PRIMARY]
GO

ALTER TABLE [sales].[order_items] ADD CONSTRAINT [PK__order_it__837942D476E9503E] PRIMARY KEY CLUSTERED (
    [order_id],
    [item_id]
    ) ON [PRIMARY]
GO

ALTER TABLE [sales].[order_items] ADD CONSTRAINT [FK__order_ite__produ__3B75D760] FOREIGN KEY ([product_id]) REFERENCES 
    [production].[products] ([product_id]) ON

DELETE CASCADE
    ON

UPDATE CASCADE
GO

ALTER TABLE [sales].[order_items] ADD CONSTRAINT [FK__order_ite__order__3A81B327] FOREIGN KEY ([order_id]) REFERENCES [sales].
    [orders] ([order_id]) ON

DELETE CASCADE
    ON

UPDATE CASCADE
GO


