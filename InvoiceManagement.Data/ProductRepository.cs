using InvoiceManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceManagement.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _dbConnection;

        public ProductRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IEnumerable<Product> GetAll()
        {
            var products = new List<Product>();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "GetProducts";
                command.CommandType = CommandType.StoredProcedure;

                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                        });
                    }
                }
                _dbConnection.Close();
            }
            return products;
        }

        public Product GetById(int id)
        {
            Product product = null;
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "GetProductById";
                command.CommandType = CommandType.StoredProcedure;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@ProductId";
                parameter.Value = id;
                parameter.DbType = DbType.Int32;
                command.Parameters.Add(parameter);

                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                        };
                    }
                }
                _dbConnection.Close();
            }
            return product;
        }

        public void Add(Product product)
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "AddProduct";
                command.CommandType = CommandType.StoredProcedure;

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "@Name";
                nameParameter.Value = product.Name;
                nameParameter.DbType = DbType.String;
                command.Parameters.Add(nameParameter);

                var descriptionParameter = command.CreateParameter();
                descriptionParameter.ParameterName = "@Description";
                descriptionParameter.Value = product.Description;
                descriptionParameter.DbType = DbType.String;
                command.Parameters.Add(descriptionParameter);

                var priceParameter = command.CreateParameter();
                priceParameter.ParameterName = "@Price";
                priceParameter.Value = product.Price;
                priceParameter.DbType = DbType.Decimal;
                command.Parameters.Add(priceParameter);

                _dbConnection.Open();
                command.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void Update(Product product)
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "UpdateProduct";
                command.CommandType = CommandType.StoredProcedure;

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@ProductId";
                idParameter.Value = product.ProductId;
                idParameter.DbType = DbType.Int32;
                command.Parameters.Add(idParameter);

                var nameParameter = command.CreateParameter();
                nameParameter.ParameterName = "@Name";
                nameParameter.Value = product.Name;
                nameParameter.DbType = DbType.String;
                command.Parameters.Add(nameParameter);

                var descriptionParameter = command.CreateParameter();
                descriptionParameter.ParameterName = "@Description";
                descriptionParameter.Value = product.Description;
                descriptionParameter.DbType = DbType.String;
                command.Parameters.Add(descriptionParameter);

                var priceParameter = command.CreateParameter();
                priceParameter.ParameterName = "@Price";
                priceParameter.Value = product.Price;
                priceParameter.DbType = DbType.Decimal;
                command.Parameters.Add(priceParameter);

                _dbConnection.Open();
                command.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
        public decimal? GetProductPrice(int productId)
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "GetProductPrice";
                command.CommandType = CommandType.StoredProcedure;

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@ProductId";
                idParameter.Value = productId;
                idParameter.DbType = DbType.Int32;
                command.Parameters.Add(idParameter);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (decimal)reader["UnitPrice"];
                    }
                }
            }
            return null;
        }

        public void Delete(int id)
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = "DeleteProduct";
                command.CommandType = CommandType.StoredProcedure;

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@ProductId";
                idParameter.Value = id;
                idParameter.DbType = DbType.Int32;
                command.Parameters.Add(idParameter);

                _dbConnection.Open();
                command.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
    }
}
