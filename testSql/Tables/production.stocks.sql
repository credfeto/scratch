CREATE TABLE [production].[stocks] (
    [store_id] [int] NOT NULL,
    [product_id] [int] NOT NULL,
    [quantity] [int] NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [production].[stocks] ADD CONSTRAINT [PK__stocks__E68284D3DB1FC6A9] PRIMARY KEY CLUSTERED (
    [store_id],
    [product_id]
    ) ON [PRIMARY]
GO

ALTER TABLE [production].[stocks] ADD CONSTRAINT [FK__stocks__product___3F466844] FOREIGN KEY ([product_id]) REFERENCES 
    [production].[products] ([product_id]) ON

DELETE CASCADE
    ON

UPDATE CASCADE
GO

ALTER TABLE [production].[stocks] ADD CONSTRAINT [FK__stocks__store_id__3E52440B] FOREIGN KEY ([store_id]) REFERENCES [sales].
    [stores] ([store_id]) ON

DELETE CASCADE
    ON

UPDATE CASCADE
GO


