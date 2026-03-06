-- Script de creación de tablas para CRM Database
-- Base de datos: CRM

USE [CRM];
GO

-- Tabla Customers
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customers](
        [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
        [CompanyName] [nvarchar](100) NOT NULL,
        [ContactFirstName] [nvarchar](50) NOT NULL,
        [ContactLastName] [nvarchar](50) NOT NULL,
        [Email] [nvarchar](150) NOT NULL,
        [Phone] [nvarchar](20) NULL,
        [AddressStreet] [nvarchar](200) NULL,
        [AddressCity] [nvarchar](100) NULL,
        [AddressState] [nvarchar](100) NULL,
        [AddressPostalCode] [nvarchar](20) NULL,
        [AddressCountry] [nvarchar](100) NULL,
        [TaxId] [nvarchar](20) NULL,
        [Status] [int] NOT NULL,
        [Type] [int] NOT NULL,
        [Notes] [nvarchar](500) NULL,
        [CreditLimit] [decimal](18, 2) NOT NULL DEFAULT 0,
        [CurrentBalance] [decimal](18, 2) NOT NULL DEFAULT 0,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        [CreatedBy] [nvarchar](100) NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1
    );

    PRINT 'Tabla Customers creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Customers ya existe';
END
GO

-- Índices para Customers
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Customer_Email' AND object_id = OBJECT_ID('Customers'))
    CREATE UNIQUE INDEX [IX_Customer_Email] ON [dbo].[Customers]([Email]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Customer_TaxId' AND object_id = OBJECT_ID('Customers'))
    CREATE UNIQUE INDEX [IX_Customer_TaxId] ON [dbo].[Customers]([TaxId]) WHERE [TaxId] IS NOT NULL;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Customer_CompanyName' AND object_id = OBJECT_ID('Customers'))
    CREATE INDEX [IX_Customer_CompanyName] ON [dbo].[Customers]([CompanyName]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Customer_Status_Active' AND object_id = OBJECT_ID('Customers'))
    CREATE INDEX [IX_Customer_Status_Active] ON [dbo].[Customers]([Status], [IsActive]);
GO

-- Tabla Products
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products](
        [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
        [SKU] [nvarchar](20) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [Category] [int] NOT NULL,
        [UnitPrice] [decimal](18, 2) NOT NULL DEFAULT 0,
        [Cost] [decimal](18, 2) NOT NULL DEFAULT 0,
        [StockQuantity] [int] NOT NULL DEFAULT 0,
        [MinimumStock] [int] NOT NULL DEFAULT 0,
        [MaximumStock] [int] NOT NULL DEFAULT 0,
        [Unit] [nvarchar](50) NOT NULL,
        [IsDiscontinued] [bit] NOT NULL DEFAULT 0,
        [DiscontinuedDate] [datetime] NULL,
        [Supplier] [nvarchar](100) NULL,
        [Weight] [decimal](18, 2) NULL,
        [Length] [decimal](18, 2) NULL,
        [Width] [decimal](18, 2) NULL,
        [Height] [decimal](18, 2) NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        [CreatedBy] [nvarchar](100) NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1
    );

    PRINT 'Tabla Products creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Products ya existe';
END
GO

-- Índices para Products
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Product_SKU' AND object_id = OBJECT_ID('Products'))
    CREATE UNIQUE INDEX [IX_Product_SKU] ON [dbo].[Products]([SKU]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Product_Name' AND object_id = OBJECT_ID('Products'))
    CREATE INDEX [IX_Product_Name] ON [dbo].[Products]([Name]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Product_Category_Active' AND object_id = OBJECT_ID('Products'))
    CREATE INDEX [IX_Product_Category_Active] ON [dbo].[Products]([Category], [IsActive]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Product_Stock' AND object_id = OBJECT_ID('Products'))
    CREATE INDEX [IX_Product_Stock] ON [dbo].[Products]([StockQuantity], [MinimumStock]);
GO

PRINT '';
PRINT '========================================';
PRINT '  Tablas e índices creados exitosamente';
PRINT '========================================';
