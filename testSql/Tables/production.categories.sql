CREATE TABLE [production].[categories] (
    [category_id] [int] NOT NULL IDENTITY(1, 1),
    [category_name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [production].[categories] ADD CONSTRAINT [PK__categori__D54EE9B4494DB9F4] PRIMARY KEY CLUSTERED ([category_id]) ON 
    [PRIMARY]
GO


