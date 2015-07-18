CREATE TABLE [dbo].[Commands]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Date] DATETIME NULL, 
    [EntityType] NVARCHAR(MAX) NULL, 
    [UserLogin] NVARCHAR(MAX) NULL, 
    [CommandType] NVARCHAR(MAX) NULL, 
    [CommandContent] NVARCHAR(MAX) NULL
)
