CREATE TABLE [sales].[orders] (
    [order_id] [int] NOT NULL IDENTITY(1, 1),
    [customer_id] [int] NULL,
    [order_status] [tinyint] NOT NULL,
    [order_date] [date] NOT NULL,
    [required_date] [date] NOT NULL,
    [shipped_date] [date] NULL,
    [store_id] [int] NOT NULL,
    [staff_id] [int] NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [sales].[orders] ADD CONSTRAINT [PK__orders__465962299C09D4CB] PRIMARY KEY CLUSTERED ([order_id]) ON [PRIMARY]
GO

ALTER TABLE [sales].[orders] ADD CONSTRAINT [FK__orders__customer__34C8D9D1] FOREIGN KEY ([customer_id]) REFERENCES [sales].
    [customers] ([customer_id]) ON

DELETE CASCADE
    ON

UPDATE CASCADE
GO

ALTER TABLE [sales].[orders] ADD CONSTRAINT [FK__orders__staff_id__36B12243] FOREIGN KEY ([staff_id]) REFERENCES [sales].
    [staffs] ([staff_id])
GO

ALTER TABLE [sales].[orders] ADD CONSTRAINT [FK__orders__store_id__35BCFE0A] FOREIGN KEY ([store_id]) REFERENCES [sales].
    [stores] ([store_id]) ON

DELETE CASCADE
    ON

UPDATE CASCADE
GO


