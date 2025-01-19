USE [master]
GO
/****** Object:  Database [InvoiceManagement]    Script Date: 01/19/2025 10:46:21 pm ******/
CREATE DATABASE [InvoiceManagement]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'InvoiceManagement', FILENAME = N'C:\Users\zeeshanilyas\InvoiceManagement.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'InvoiceManagement_log', FILENAME = N'C:\Users\zeeshanilyas\InvoiceManagement_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [InvoiceManagement] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [InvoiceManagement].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [InvoiceManagement] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [InvoiceManagement] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [InvoiceManagement] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [InvoiceManagement] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [InvoiceManagement] SET ARITHABORT OFF 
GO
ALTER DATABASE [InvoiceManagement] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [InvoiceManagement] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [InvoiceManagement] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [InvoiceManagement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [InvoiceManagement] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [InvoiceManagement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [InvoiceManagement] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [InvoiceManagement] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [InvoiceManagement] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [InvoiceManagement] SET  ENABLE_BROKER 
GO
ALTER DATABASE [InvoiceManagement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [InvoiceManagement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [InvoiceManagement] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [InvoiceManagement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [InvoiceManagement] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [InvoiceManagement] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [InvoiceManagement] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [InvoiceManagement] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [InvoiceManagement] SET  MULTI_USER 
GO
ALTER DATABASE [InvoiceManagement] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [InvoiceManagement] SET DB_CHAINING OFF 
GO
ALTER DATABASE [InvoiceManagement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [InvoiceManagement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [InvoiceManagement] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [InvoiceManagement] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [InvoiceManagement] SET QUERY_STORE = OFF
GO
USE [InvoiceManagement]
GO
/****** Object:  Table [dbo].[InvoiceDetails]    Script Date: 01/19/2025 10:46:21 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceDetails](
	[InvoiceDetailId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[TotalPrice]  AS ([Quantity]*[UnitPrice]) PERSISTED,
PRIMARY KEY CLUSTERED 
(
	[InvoiceDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InvoicePayments]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoicePayments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[PaymentDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[InvoiceId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [varchar](255) NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](500) NULL,
	[Price] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InvoiceDetails]  WITH CHECK ADD FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([InvoiceId])
GO
ALTER TABLE [dbo].[InvoiceDetails]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[InvoicePayments]  WITH CHECK ADD FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([InvoiceId])
GO
/****** Object:  StoredProcedure [dbo].[AddInvoice]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddInvoice]
    @CustomerName VARCHAR(255),
    @InvoiceDate DATETIME
AS
BEGIN
    INSERT INTO Invoices (CustomerName, InvoiceDate)
    VALUES (@CustomerName, @InvoiceDate);

    -- Return the newly inserted InvoiceId
    SELECT SCOPE_IDENTITY() AS InvoiceId;
END;


GO
/****** Object:  StoredProcedure [dbo].[AddInvoiceDetail]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddInvoiceDetail]
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
/****** Object:  StoredProcedure [dbo].[AddInvoicePayment]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddInvoicePayment]
    @InvoiceId INT,
    @PaymentDate DATETIME,
    @Amount DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO InvoicePayments (InvoiceId, PaymentDate, Amount)
    VALUES (@InvoiceId, @PaymentDate, @Amount);
END;
GO
/****** Object:  StoredProcedure [dbo].[AddProduct]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddProduct]
    @Name VARCHAR(255),
    @Description VARCHAR(500),
    @Price DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO Products (Name, Description, Price)
    VALUES (@Name, @Description, @Price);
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteInvoice]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteInvoice]
    @InvoiceId INT
AS
BEGIN
    DELETE FROM Invoices
    WHERE InvoiceId = @InvoiceId;
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteInvoiceDetail]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteInvoiceDetail]
    @InvoiceDetailId INT
AS
BEGIN
    DELETE FROM InvoiceDetails
    WHERE InvoiceDetailId = @InvoiceDetailId;
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteInvoicePayment]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteInvoicePayment]
    @PaymentId INT
AS
BEGIN
    DELETE FROM InvoicePayments
    WHERE PaymentId = @PaymentId;
END;
GO
/****** Object:  StoredProcedure [dbo].[DeleteInvoiceWithDetails]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteInvoiceWithDetails]
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
GO
/****** Object:  StoredProcedure [dbo].[DeleteProduct]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteProduct]
    @ProductId INT
AS
BEGIN
    DELETE FROM Products
    WHERE ProductId = @ProductId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetAllPayments]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllPayments]
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
/****** Object:  StoredProcedure [dbo].[GetInvoiceById]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetInvoiceById]
    @InvoiceId INT
AS
BEGIN
    SELECT * FROM Invoices WHERE InvoiceId = @InvoiceId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetInvoiceDetailsByInvoiceId]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetInvoiceDetailsByInvoiceId]
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
/****** Object:  StoredProcedure [dbo].[GetInvoicePaymentsByInvoiceId]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInvoicePaymentsByInvoiceId]
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
/****** Object:  StoredProcedure [dbo].[GetInvoices]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetInvoices]
AS
BEGIN
    -- Select invoices with calculated total amount from InvoiceDetails
    SELECT 
        i.InvoiceId,
        i.CustomerName,
        i.InvoiceDate,
        ISNULL(SUM(id.Quantity * id.UnitPrice), 0) AS TotalAmount
    FROM 
        Invoices i
    LEFT JOIN 
        InvoiceDetails id ON i.InvoiceId = id.InvoiceId
    GROUP BY 
        i.InvoiceId, i.CustomerName, i.InvoiceDate
    ORDER BY 
        i.InvoiceId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetProductById]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetProductById]
    @ProductId INT
AS
BEGIN
    SELECT * FROM Products WHERE ProductId = @ProductId;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetProductPrice]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetProductPrice]
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
GO
/****** Object:  StoredProcedure [dbo].[GetProducts]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetProducts]
AS
BEGIN
    SELECT * FROM Products;
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdateInvoice]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateInvoice]
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
/****** Object:  StoredProcedure [dbo].[UpdateInvoiceDetail]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateInvoiceDetail]
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
/****** Object:  StoredProcedure [dbo].[UpdateInvoicePayment]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateInvoicePayment]
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
/****** Object:  StoredProcedure [dbo].[UpdateProduct]    Script Date: 01/19/2025 10:46:22 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateProduct]
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
USE [master]
GO
ALTER DATABASE [InvoiceManagement] SET  READ_WRITE 
GO
