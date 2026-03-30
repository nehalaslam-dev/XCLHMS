-- =============================================
-- MISSING TABLES FOR STOCK INVENTORY
-- =============================================

CREATE TABLE [dbo].[Manufactures] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ManufactureName] NVARCHAR(max),
    [Description] NVARCHAR(max)
);

CREATE TABLE [dbo].[Brands] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [BrandName] NVARCHAR(max),
    [Description] NVARCHAR(max)
);

CREATE TABLE [dbo].[Issuances] (
    [SNO] INT IDENTITY(1,1) PRIMARY KEY,
    [Head] NVARCHAR(max),
    [ProductId] INT,
    [Qty] DECIMAL(18,2),
    [Date] DATETIME
);
GO
