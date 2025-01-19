using InvoiceManagement.Data;
using InvoiceManagement.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Data;
using Xunit;

public class InvoicesRepositoryTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly InvoicesRepository _repository;

    public InvoicesRepositoryTests()
    {
        // Mock IConfiguration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration
     .Setup(config => config["ConnectionStrings:DefaultConnection"])
     .Returns("Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;");

        // Mock IDbConnection
        _mockDbConnection = new Mock<IDbConnection>();

        // Create the repository
        _repository = new InvoicesRepository(_mockConfiguration.Object);
    }

    [Fact]
    public void GetInvoiceDetailsByInvoiceId_ShouldReturnListOfInvoiceDetails()
    {
        // Arrange
        var mockDetails = new List<InvoiceDetail>
        {
            new InvoiceDetail { InvoiceDetailId = 1, InvoiceId = 1, ProductId = 1, ProductName = "Product A", Quantity = 2, UnitPrice = 100.0M },
            new InvoiceDetail { InvoiceDetailId = 2, InvoiceId = 1, ProductId = 2, ProductName = "Product B", Quantity = 1, UnitPrice = 200.0M }
        };

        var mockReader = new Mock<IDataReader>();
        mockReader.SetupSequence(reader => reader.Read())
            .Returns(true) // Row 1
            .Returns(true) // Row 2
            .Returns(false); // End of data
        mockReader.Setup(reader => reader["InvoiceDetailId"]).Returns(1);
        mockReader.Setup(reader => reader["InvoiceId"]).Returns(1);
        mockReader.Setup(reader => reader["ProductId"]).Returns(1);
        mockReader.Setup(reader => reader["ProductName"]).Returns("Product A");
        mockReader.Setup(reader => reader["Quantity"]).Returns(2);
        mockReader.Setup(reader => reader["UnitPrice"]).Returns(100.0M);

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        var details = _repository.GetInvoiceDetailsByInvoiceId(1);

        // Assert
        Assert.NotNull(details);
        Assert.Equal(2, details.Count());
        Assert.Equal("Product A", details.First().ProductName);
        Assert.Equal(100.0M, details.First().UnitPrice);
    }

    [Fact]
    public void AddInvoiceDetail_ShouldInsertInvoiceDetail()
    {
        // Arrange
        var detail = new InvoiceDetail { InvoiceId = 1, ProductId = 1, Quantity = 2, UnitPrice = 100.0M };

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.AddInvoiceDetail(detail);

        // Assert
        mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
    }

    [Fact]
    public void UpdateInvoiceDetail_ShouldUpdateInvoiceDetail()
    {
        // Arrange
        var detail = new InvoiceDetail { InvoiceDetailId = 1, Quantity = 3, UnitPrice = 150.0M };

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.UpdateInvoiceDetail(detail);

        // Assert
        mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
    }

    [Fact]
    public void DeleteInvoiceDetail_ShouldDeleteInvoiceDetail()
    {
        // Arrange
        var detailId = 1;

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.DeleteInvoiceDetail(detailId);

        // Assert
        mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
    }

    [Fact]
    public void GetInvoicePaymentsByInvoiceId_ShouldReturnListOfPayments()
    {
        // Arrange
        var mockPayments = new List<InvoicePayment>
        {
            new InvoicePayment { PaymentId = 1, InvoiceId = 1, PaymentDate = new DateTime(2023, 1, 1), Amount = 500.0M },
            new InvoicePayment { PaymentId = 2, InvoiceId = 1, PaymentDate = new DateTime(2023, 1, 2), Amount = 300.0M }
        };

        var mockReader = new Mock<IDataReader>();
        mockReader.SetupSequence(reader => reader.Read())
            .Returns(true) // Row 1
            .Returns(true) // Row 2
            .Returns(false); // End of data
        mockReader.Setup(reader => reader["PaymentId"]).Returns(1);
        mockReader.Setup(reader => reader["InvoiceId"]).Returns(1);
        mockReader.Setup(reader => reader["PaymentDate"]).Returns(new DateTime(2023, 1, 1));
        mockReader.Setup(reader => reader["Amount"]).Returns(500.0M);

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        var payments = _repository.GetInvoicePaymentsByInvoiceId(1);

        // Assert
        Assert.NotNull(payments);
        Assert.Equal(2, payments.Count());
        Assert.Equal(500.0M, payments.First().Amount);
    }

    [Fact]
    public void AddInvoicePayment_ShouldInsertPayment()
    {
        // Arrange
        var payment = new InvoicePayment { InvoiceId = 1, PaymentDate = new DateTime(2023, 1, 1), Amount = 500.0M };

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.AddInvoicePayment(payment);

        // Assert
        mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
    }
}
