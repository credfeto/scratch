CREATE TABLE [sales].[staffs]
(
[staff_id] [int] NOT NULL IDENTITY(1, 1),
[first_name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[last_name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[email] [varchar] (255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[phone] [varchar] (25) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[active] [tinyint] NOT NULL,
[store_id] [int] NOT NULL,
[manager_id] [int] NULL
) ON [PRIMARY]
GO
ALTER TABLE [sales].[staffs] ADD CONSTRAINT [PK__staffs__1963DD9CE366003B] PRIMARY KEY CLUSTERED  ([staff_id]) ON [PRIMARY]
GO
ALTER TABLE [sales].[staffs] ADD CONSTRAINT [UQ__staffs__AB6E6164E60DF5B8] UNIQUE NONCLUSTERED  ([email]) ON [PRIMARY]
GO
ALTER TABLE [sales].[staffs] ADD CONSTRAINT [FK__staffs__manager___31EC6D26] FOREIGN KEY ([manager_id]) REFERENCES [sales].[staffs] ([staff_id])
GO
ALTER TABLE [sales].[staffs] ADD CONSTRAINT [FK__staffs__store_id__30F848ED] FOREIGN KEY ([store_id]) REFERENCES [sales].[stores] ([store_id]) ON DELETE CASCADE ON UPDATE CASCADE
GO
