CREATE TABLE [sales].[customers] (
    [customer_id] [int] NOT NULL IDENTITY(1, 1),
    [first_name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [last_name] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [phone] [varchar](25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [email] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [street] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [city] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [state] [varchar](25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [zip_code] [varchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
    ) ON [PRIMARY]
GO

ALTER TABLE [sales].[customers] ADD CONSTRAINT [PK__customer__CD65CB858B43AFD4] PRIMARY KEY CLUSTERED ([customer_id]) ON 
    [PRIMARY]
GO


