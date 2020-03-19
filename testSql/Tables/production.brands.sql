CREATE TABLE [production].[brands] (
    [brand_id] [int] NOT NULL IDENTITY(1, 1),
    [brand_name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [production].[brands] ADD CONSTRAINT [PK__brands__5E5A8E27FFA26A8B] PRIMARY KEY CLUSTERED ([brand_id]) ON [PRIMARY]
GO


