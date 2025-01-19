-- Step 1: Create or Update the Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'InvoiceManagement')
BEGIN
    CREATE DATABASE InvoiceManagement;
END;
GO

USE InvoiceManagement;
GO

-- Step 2: Drop Tables in Correct Order to Handle Foreign Key Constraints
IF OBJECT_ID('InvoiceDetails', 'U') IS NOT NULL
    DROP TABLE InvoiceDetails;
GO

IF OBJECT_ID('InvoicePayments', 'U') IS NOT NULL
    DROP TABLE InvoicePayments;
GO

IF OBJECT_ID('Invoices', 'U') IS NOT NULL
    DROP TABLE Invoices;
GO

IF OBJECT_ID('Products', 'U') IS NOT NULL
    DROP TABLE Products;
GO

-- Step 3: Create Tables
-- 3.1 Products Table
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(500),
    Price DECIMAL(18, 2) NOT NULL
);
GO

-- 3.2 Invoices Table
CREATE TABLE Invoices (
    InvoiceId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName VARCHAR(255) NOT NULL,
    InvoiceDate DATETIME NOT NULL
);
GO

-- 3.3 InvoiceDetails Table
CREATE TABLE InvoiceDetails (
    InvoiceDetailId INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    TotalPrice AS (Quantity * UnitPrice) PERSISTED,
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO

-- 3.4 InvoicePayments Table
CREATE TABLE InvoicePayments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceId INT NOT NULL,
    PaymentDate DATETIME NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId)
);
GO

-- Step 4: Create or Update Stored Procedures
-- 4.1 CRUD for Products
IF OBJECT_ID('AddProduct', 'P') IS NOT NULL
    DROP PROCEDURE AddProduct;
GO

CREATE PROCEDURE AddProduct
    @Name VARCHAR(255),
    @Description VARCHAR(500),
    @Price DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO Products (Name, Description, Price)
    VALUES (@Name, @Description, @Price);
END;
GO

IF OBJECT_ID('GetProductById', 'P') IS NOT NULL
    DROP PROCEDURE GetProductById;
GO

CREATE PROCEDURE GetProductById
    @ProductId INT
AS
BEGIN
    SELECT * FROM Products WHERE ProductId = @ProductId;
END;
GO

IF OBJECT_ID('GetProducts', 'P') IS NOT NULL
    DROP PROCEDURE GetProducts;
GO

CREATE PROCEDURE GetProducts
AS
BEGIN
    SELECT * FROM Products;
END;
GO

IF OBJECT_ID('UpdateProduct', 'P') IS NOT NULL
    DROP PROCEDURE UpdateProduct;
GO

CREATE PROCEDURE UpdateProduct
    @ProductId INT,
    @Name VARCHAR(255),
    @Description VARCHAR(500),
    @Price DECIMAL(18, 2)
AS
BEGIN
    UPDATE Products
    SET Name = @Name, Description = @Description, Price = @Price
    WHERE ProductId = @ProductId;
END;
GO

IF OBJECT_ID('DeleteProduct', 'P') IS NOT NULL
    DROP PROCEDURE DeleteProduct;
GO

CREATE PROCEDURE DeleteProduct
    @ProductId INT
AS
BEGIN
    DELETE FROM Products
    WHERE ProductId = @ProductId;
END;
GO

-- 4.2 CRUD for Invoices
IF OBJECT_ID('AddInvoice', 'P') IS NOT NULL
    DROP PROCEDURE AddInvoice;
GO

CREATE PROCEDURE AddInvoice
    @CustomerName VARCHAR(255),
    @InvoiceDate DATETIME
AS
BEGIN
    INSERT INTO Invoices (CustomerName, InvoiceDate)
    VALUES (@CustomerName, @InvoiceDate);
END;
GO

IF OBJECT_ID('GetInvoices', 'P') IS NOT NULL
    DROP PROCEDURE GetInvoices;
GO

CREATE PROCEDURE GetInvoices
AS
BEGIN
    SELECT * FROM Invoices;
END;
GO

IF OBJECT_ID('UpdateInvoice', 'P') IS NOT NULL
    DROP PROCEDURE UpdateInvoice;
GO

CREATE PROCEDURE UpdateInvoice
    @InvoiceId INT,
    @CustomerName VARCHAR(255),
    @InvoiceDate DATETIME
AS
BEGIN
    UPDATE Invoices
    SET CustomerName = @CustomerName, InvoiceDate = @InvoiceDate
    WHERE InvoiceId = @InvoiceId;
END;
GO

IF OBJECT_ID('DeleteInvoice', 'P') IS NOT NULL
    DROP PROCEDURE DeleteInvoice;
GO

CREATE PROCEDURE DeleteInvoice
    @InvoiceId INT
AS
BEGIN
    DELETE FROM Invoices
    WHERE InvoiceId = @InvoiceId;
END;
GO

-- 4.3 CRUD for InvoiceDetails
IF OBJECT_ID('AddInvoiceDetail', 'P') IS NOT NULL
    DROP PROCEDURE AddInvoiceDetail;
GO

CREATE PROCEDURE AddInvoiceDetail
    @InvoiceId INT,
    @ProductId INT,
    @Quantity INT,
    @UnitPrice DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO InvoiceDetails (InvoiceId, ProductId, Quantity, UnitPrice)
    VALUES (@InvoiceId, @ProductId, @Quantity, @UnitPrice);
END;
GO

IF OBJECT_ID('GetInvoiceDetailsByInvoiceId', 'P') IS NOT NULL
    DROP PROCEDURE GetInvoiceDetailsByInvoiceId;
GO

CREATE PROCEDURE GetInvoiceDetailsByInvoiceId
    @InvoiceId INT
AS
BEGIN
    SELECT d.InvoiceDetailId, d.InvoiceId, d.ProductId, p.Name AS ProductName, 
           d.Quantity, d.UnitPrice, d.TotalPrice
    FROM InvoiceDetails d
    INNER JOIN Products p ON d.ProductId = p.ProductId
    WHERE d.InvoiceId = @InvoiceId;
END;
GO

IF OBJECT_ID('UpdateInvoiceDetail', 'P') IS NOT NULL
    DROP PROCEDURE UpdateInvoiceDetail;
GO

CREATE PROCEDURE UpdateInvoiceDetail
    @InvoiceDetailId INT,
    @Quantity INT,
    @UnitPrice DECIMAL(18, 2)
AS
BEGIN
    UPDATE InvoiceDetails
    SET Quantity = @Quantity, UnitPrice = @UnitPrice
    WHERE InvoiceDetailId = @InvoiceDetailId;
END;
GO

IF OBJECT_ID('DeleteInvoiceDetail', 'P') IS NOT NULL
    DROP PROCEDURE DeleteInvoiceDetail;
GO

CREATE PROCEDURE DeleteInvoiceDetail
    @InvoiceDetailId INT
AS
BEGIN
    DELETE FROM InvoiceDetails
    WHERE InvoiceDetailId = @InvoiceDetailId;
END;
GO

-- 4.4 CRUD for InvoicePayments
IF OBJECT_ID('AddInvoicePayment', 'P') IS NOT NULL
    DROP PROCEDURE AddInvoicePayment;
GO

CREATE PROCEDURE AddInvoicePayment
    @InvoiceId INT,
    @PaymentDate DATETIME,
    @Amount DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO InvoicePayments (InvoiceId, PaymentDate, Amount)
    VALUES (@InvoiceId, @PaymentDate, @Amount);
END;
GO

IF OBJECT_ID('UpdateInvoicePayment', 'P') IS NOT NULL
    DROP PROCEDURE UpdateInvoicePayment;
GO

CREATE PROCEDURE UpdateInvoicePayment
    @PaymentId INT,
    @PaymentDate DATETIME,
    @Amount DECIMAL(18, 2)
AS
BEGIN
    UPDATE InvoicePayments
    SET PaymentDate = @PaymentDate, Amount = @Amount
    WHERE PaymentId = @PaymentId;
END;
GO

IF OBJECT_ID('DeleteInvoicePayment', 'P') IS NOT NULL
    DROP PROCEDURE DeleteInvoicePayment;
GO

CREATE PROCEDURE DeleteInvoicePayment
    @PaymentId INT
AS
BEGIN
    DELETE FROM InvoicePayments
    WHERE PaymentId = @PaymentId;
END;
GO

CREATE PROCEDURE GetInvoicePaymentsByInvoiceId
    @InvoiceId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        PaymentId,
        InvoiceId,
        PaymentDate,
        Amount
    FROM InvoicePayments
    WHERE InvoiceId = @InvoiceId;
END;
GO





select * from Products;

-- Insert data into Products table
INSERT INTO Products (Name, Description, Price) VALUES 
('Product A', 'Description for Product A', 15.50),
('Product B', 'Description for Product B', 25.00),
('Product C', 'Description for Product C', 30.75),
('Product D', 'Description for Product D', 10.00);

-- Insert data into Invoices table
INSERT INTO Invoices (CustomerName, InvoiceDate) VALUES 
('John Doe', GETDATE()),
('Jane Smith', DATEADD(DAY, -1, GETDATE())),
('Alice Johnson', DATEADD(DAY, -2, GETDATE())),
('Bob Brown', DATEADD(DAY, -3, GETDATE()));

-- Insert data into InvoiceDetails table
-- Assuming InvoiceIds are 1, 2, 3, 4 and ProductIds are 1, 2, 3, 4
INSERT INTO InvoiceDetails (InvoiceId, ProductId, Quantity, UnitPrice) VALUES 
(1, 1, 2, 15.50),  -- 2 units of Product A for Invoice 1
(1, 2, 1, 25.00),  -- 1 unit of Product B for Invoice 1
(2, 3, 3, 30.75),  -- 3 units of Product C for Invoice 2
(2, 4, 2, 10.00),  -- 2 units of Product D for Invoice 2
(3, 1, 1, 15.50),  -- 1 unit of Product A for Invoice 3
(3, 3, 2, 30.75),  -- 2 units of Product C for Invoice 3
(4, 2, 4, 25.00),  -- 4 units of Product B for Invoice 4
(4, 4, 1, 10.00);  -- 1 unit of Product D for Invoice 4

-- Insert data into InvoicePayments table
-- Assuming InvoiceIds are 1, 2, 3, 4
INSERT INTO InvoicePayments (InvoiceId, PaymentDate, Amount) VALUES 
(1, GETDATE(), 56.00),  -- Payment for Invoice 1
(2, DATEADD(DAY, -1, GETDATE()), 122.25), -- Payment for Invoice 2
(3, DATEADD(DAY, -2, GETDATE()), 77.00),  -- Payment for Invoice 3
(4, DATEADD(DAY, -3, GETDATE()), 110.00); -- Payment for Invoice 4
	


CREATE PROCEDURE GetAllPayments
    @InvoiceId INT = NULL,         -- Optional filter by InvoiceId
    @PaymentId INT = NULL,         -- Optional filter by PaymentId
    @PaymentDate DATE = NULL,      -- Optional filter by PaymentDate
    @Amount DECIMAL(18, 2) = NULL  -- Optional filter by Amount
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        PaymentId,
        InvoiceId,
        PaymentDate,
        Amount
    FROM 
        InvoicePayments
    WHERE 
        (@InvoiceId IS NULL OR InvoiceId = @InvoiceId)
        AND (@PaymentId IS NULL OR PaymentId = @PaymentId)
        AND (@PaymentDate IS NULL OR CAST(PaymentDate AS DATE) = @PaymentDate)
        AND (@Amount IS NULL OR Amount = @Amount)
    ORDER BY 
        PaymentDate DESC; -- Optional: You can order by any column
END;


GO

CREATE PROCEDURE DeleteInvoiceWithDetails
    @InvoiceId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Delete all related payments
        DELETE FROM InvoicePayments
        WHERE InvoiceId = @InvoiceId;

        -- Delete all related invoice details
        DELETE FROM InvoiceDetails
        WHERE InvoiceId = @InvoiceId;

        -- Delete the invoice
        DELETE FROM Invoices
        WHERE InvoiceId = @InvoiceId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Optionally, re-throw the error to handle it in the application
        THROW;
    END CATCH
END;



CREATE PROCEDURE GetProductPrice
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ProductId,
        Name AS ProductName,
        Price AS UnitPrice
    FROM 
        Products
    WHERE 
        ProductId = @ProductId;
END;
