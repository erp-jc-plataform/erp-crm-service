-- Script para insertar datos de prueba en CRM
USE [CRM];
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Insertar clientes de prueba
PRINT 'Insertando clientes de prueba...';

-- Cliente 1: Empresa de Tecnología
INSERT INTO [dbo].[Customers] 
    ([Id], [CompanyName], [ContactFirstName], [ContactLastName], [Email], [Phone], 
     [AddressStreet], [AddressCity], [AddressState], [AddressPostalCode], [AddressCountry],
     [TaxId], [Status], [Type], [CreditLimit], [CurrentBalance], [Notes], 
     [CreatedAt], [CreatedBy], [IsActive])
VALUES 
    (NEWID(), 'TechSolutions SA', 'Juan', 'Pérez', 'juan.perez@techsolutions.com', '+34-912-345-678',
     'Calle Mayor 15', 'Madrid', 'Madrid', '28001', 'España',
     'B12345678', 1, 2, 50000.00, 0.00, 'Cliente corporativo de tecnología',
     GETDATE(), 'System', 1);

-- Cliente 2: Empresa de Retail
INSERT INTO [dbo].[Customers] 
    ([Id], [CompanyName], [ContactFirstName], [ContactLastName], [Email], [Phone], 
     [AddressStreet], [AddressCity], [AddressState], [AddressPostalCode], [AddressCountry],
     [TaxId], [Status], [Type], [CreditLimit], [CurrentBalance], [Notes], 
     [CreatedAt], [CreatedBy], [IsActive])
VALUES 
    (NEWID(), 'Retail Express', 'María', 'González', 'maria.gonzalez@retailexpress.com', '+34-933-456-789',
     'Avenida Diagonal 500', 'Barcelona', 'Barcelona', '08029', 'España',
     'B87654321', 1, 2, 75000.00, 0.00, 'Cliente de retail con alta rotación',
     GETDATE(), 'System', 1);

-- Cliente 3: Autónomo
INSERT INTO [dbo].[Customers] 
    ([Id], [CompanyName], [ContactFirstName], [ContactLastName], [Email], [Phone], 
     [AddressStreet], [AddressCity], [AddressState], [AddressPostalCode], [AddressCountry],
     [TaxId], [Status], [Type], [CreditLimit], [CurrentBalance], [Notes], 
     [CreatedAt], [CreatedBy], [IsActive])
VALUES 
    (NEWID(), 'Consultora García', 'Carlos', 'García', 'carlos@consultoragarcia.com', '+34-954-567-890',
     'Calle Sierpes 25', 'Sevilla', 'Sevilla', '41004', 'España',
     'X1234567A', 1, 1, 10000.00, 0.00, 'Consultor independiente',
     GETDATE(), 'System', 1);

PRINT 'Clientes insertados correctamente';
GO

-- Insertar productos de prueba
PRINT 'Insertando productos de prueba...';

-- Producto 1: Laptop
INSERT INTO [dbo].[Products]
    ([Id], [SKU], [Name], [Description], [Category], [UnitPrice], [Cost],
     [StockQuantity], [MinimumStock], [MaximumStock], [Unit], [Supplier],
     [Weight], [Length], [Width], [Height], [IsDiscontinued],
     [CreatedAt], [CreatedBy], [IsActive])
VALUES
    (NEWID(), 'LAP-001', 'Laptop Dell XPS 15', 'Laptop profesional de alto rendimiento', 1, 1499.99, 1200.00,
     25, 5, 100, 'Unidad', 'Dell Inc.',
     2.5, 35.0, 24.0, 2.0, 0,
     GETDATE(), 'System', 1);

-- Producto 2: Monitor
INSERT INTO [dbo].[Products]
    ([Id], [SKU], [Name], [Description], [Category], [UnitPrice], [Cost],
     [StockQuantity], [MinimumStock], [MaximumStock], [Unit], [Supplier],
     [Weight], [Length], [Width], [Height], [IsDiscontinued],
     [CreatedAt], [CreatedBy], [IsActive])
VALUES
    (NEWID(), 'MON-001', 'Monitor LG 27" 4K', 'Monitor profesional 4K IPS', 1, 399.99, 300.00,
     45, 10, 200, 'Unidad', 'LG Electronics',
     5.2, 61.0, 45.0, 18.0, 0,
     GETDATE(), 'System', 1);

-- Producto 3: Teclado
INSERT INTO [dbo].[Products]
    ([Id], [SKU], [Name], [Description], [Category], [UnitPrice], [Cost],
     [StockQuantity], [MinimumStock], [MaximumStock], [Unit], [Supplier],
     [Weight], [Length], [Width], [Height], [IsDiscontinued],
     [CreatedAt], [CreatedBy], [IsActive])
VALUES
    (NEWID(), 'KEY-001', 'Teclado Mecánico RGB', 'Teclado mecánico gaming con iluminación RGB', 1, 89.99, 50.00,
     120, 20, 500, 'Unidad', 'Logitech',
     1.2, 44.0, 13.0, 3.5, 0,
     GETDATE(), 'System', 1);

-- Producto 4: Mouse
INSERT INTO [dbo].[Products]
    ([Id], [SKU], [Name], [Description], [Category], [UnitPrice], [Cost],
     [StockQuantity], [MinimumStock], [MaximumStock], [Unit], [Supplier],
     [Weight], [Length], [Width], [Height], [IsDiscontinued],
     [CreatedAt], [CreatedBy], [IsActive])
VALUES
    (NEWID(), 'MOU-001', 'Mouse Inalámbrico MX Master 3', 'Mouse ergonómico inalámbrico', 1, 99.99, 60.00,
     80, 15, 300, 'Unidad', 'Logitech',
     0.14, 12.5, 8.4, 5.1, 0,
     GETDATE(), 'System', 1);

-- Producto 5: Impresora (Stock bajo)
INSERT INTO [dbo].[Products]
    ([Id], [SKU], [Name], [Description], [Category], [UnitPrice], [Cost],
     [StockQuantity], [MinimumStock], [MaximumStock], [Unit], [Supplier],
     [Weight], [Length], [Width], [Height], [IsDiscontinued],
     [CreatedAt], [CreatedBy], [IsActive])
VALUES
    (NEWID(), 'PRI-001', 'Impresora HP LaserJet Pro', 'Impresora láser monocromática', 1, 249.99, 180.00,
     3, 5, 50, 'Unidad', 'HP Inc.',
     7.5, 37.0, 36.0, 18.0, 0,
     GETDATE(), 'System', 1);

PRINT 'Productos insertados correctamente';
GO

-- Mostrar resumen
PRINT '';
PRINT '========================================';
PRINT '  Datos de prueba insertados';
PRINT '========================================';
PRINT '';

DECLARE @CustomerCount INT, @ProductCount INT;

SELECT @CustomerCount = COUNT(*) FROM [dbo].[Customers];
SELECT @ProductCount = COUNT(*) FROM [dbo].[Products];

PRINT 'Total de Clientes: ' + CAST(@CustomerCount AS VARCHAR);
PRINT 'Total de Productos: ' + CAST(@ProductCount AS VARCHAR);

-- Mostrar productos con stock bajo
PRINT '';
PRINT 'Productos con stock bajo (Stock < Mínimo):';
SELECT [Name], [SKU], [StockQuantity], [MinimumStock]
FROM [dbo].[Products]
WHERE [StockQuantity] < [MinimumStock];
