using InvoiceManagement.Data;
using InvoiceManagement.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;

public class ProductRepositoryTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<SqlConnection> _mockSqlConnection;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        // Mock IConfiguration
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration
            .Setup(config => config["ConnectionStrings:DefaultConnection"])
            .Returns("Server=(localdb)\\mssqllocaldb;Database=InvoiceManagement;Trusted_Connection=True;");

        // Use a real SqlConnection with the mocked configuration
        var connectionString = _mockConfiguration.Object["ConnectionStrings:DefaultConnection"];
        var realDbConnection = new SqlConnection(connectionString);

        // Instantiate repository with real SqlConnection
        _repository = new ProductRepository(realDbConnection);
    }



    [Fact]
    public void GetAll_ShouldReturnListOfProducts()
    {
        // Arrange
        var mockProducts = new List<Product>
    {
        new Product { ProductId = 1, Name = "Product A", Description = "Description A", Price = 10.0M },
        new Product { ProductId = 2, Name = "Product B", Description = "Description B", Price = 20.0M }
    };

        var mockReader = new Mock<IDataReader>();
        mockReader.SetupSequence(reader => reader.Read())
            .Returns(true)  // Row 1
            .Returns(true)  // Row 2
            .Returns(false); // End of data
        mockReader.Setup(reader => reader["ProductId"]).Returns(1);
        mockReader.Setup(reader => reader["Name"]).Returns("Product A");
        mockReader.Setup(reader => reader["Description"]).Returns("Description A");
        mockReader.Setup(reader => reader["Price"]).Returns(10.0M);

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        var products = _repository.GetAll();

        // Assert
        Assert.NotNull(products); // Ensure the collection is not null.
        Assert.NotEmpty(products); // Ensure the collection has at least one element.
        //Assert.Equal(2, products.Count()); // Use Count() instead of Count
        //Assert.Equal("Product A", products.First().Name);
        //Assert.Equal(10.0M, products.First().Price);
    }


    [Fact]
    public void GetById_ShouldReturnSingleProduct()
    {
        // Arrange
        var mockProduct = new Product { ProductId = 1, Name = "Product A", Description = "Description A", Price = 10.0M };

        var mockReader = new Mock<IDataReader>();
        mockReader.SetupSequence(reader => reader.Read())
            .Returns(true)
            .Returns(false);
        mockReader.Setup(reader => reader["ProductId"]).Returns(mockProduct.ProductId);
        mockReader.Setup(reader => reader["Name"]).Returns(mockProduct.Name);
        mockReader.Setup(reader => reader["Description"]).Returns(mockProduct.Description);
        mockReader.Setup(reader => reader["Price"]).Returns(mockProduct.Price);

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        var product = _repository.GetById(1);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Product A", product.Name);
        //Assert.Equal(10.0M, product.Price);
    }

    [Fact]
    public void Add_ShouldInsertProduct()
    {
        // Arrange
        var product = new Product { Name = "Product E", Description = "Description E", Price = 30.0M };

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);
        mockCommand.Setup(cmd => cmd.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.Add(product);

        // Assert
        //mockConnection.Verify(conn => conn.CreateCommand(), Times.Once); // Verify CreateCommand was called
        //mockCommand.Verify(cmd => cmd.CreateParameter(), Times.Exactly(3)); // Verify parameters were created
        //mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once); // Verify ExecuteNonQuery was called
        mockConnection.Verify(conn => conn.Open(), Times.Once); // Verify connection was opened
        mockConnection.Verify(conn => conn.Close(), Times.Once); // Verify connection was closed
    }


    [Fact]
    public void Update_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Updated Product", Description = "Updated Description", Price = 50.0M };

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.Update(product);

        // Assert
        mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
    }

    [Fact]
    public void Delete_ShouldDeleteProduct()
    {
        // Arrange
        var productId = 1;

        var mockCommand = new Mock<IDbCommand>();
        mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

        var mockConnection = new Mock<IDbConnection>();
        mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);

        // Act
        _repository.Delete(productId);

        // Assert
        mockCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Once);
    }
}