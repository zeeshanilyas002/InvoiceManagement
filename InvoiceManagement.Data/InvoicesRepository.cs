using InvoiceManagement.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Data
{
    public class InvoicesRepository : IInvoicesRepository
    {
        private readonly string _connectionString;

        public InvoicesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Master: Invoices
        public IEnumerable<Invoice> GetAllInvoices()
        {
            var invoices = new List<Invoice>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetInvoices", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            invoices.Add(new Invoice
                            {
                                InvoiceId = (int)reader["InvoiceId"],
                                CustomerName = reader["CustomerName"].ToString(),
                                InvoiceDate = (DateTime)reader["InvoiceDate"]
                            });
                        }
                    }
                }
            }
            return invoices;
        }

        public Invoice GetInvoiceById(int id)
        {
            Invoice invoice = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetInvoiceById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            invoice = new Invoice
                            {
                                InvoiceId = (int)reader["InvoiceId"],
                                CustomerName = reader["CustomerName"].ToString(),
                                InvoiceDate = (DateTime)reader["InvoiceDate"]
                            };
                        }
                    }
                }
            }
            return invoice;
        }

        public int AddInvoice(Invoice invoice)
        {
            int invoiceId;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("AddInvoice", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerName", invoice.CustomerName);
                    command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);

                    invoiceId = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return invoiceId;
        }

        public void UpdateInvoice(Invoice invoice)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdateInvoice", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", invoice.InvoiceId);
                    command.Parameters.AddWithValue("@CustomerName", invoice.CustomerName);
                    command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteInvoice(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DeleteInvoice", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", id);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Details: InvoiceDetails
        public IEnumerable<InvoiceDetail> GetInvoiceDetailsByInvoiceId(int invoiceId)
        {
            var details = new List<InvoiceDetail>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetInvoiceDetailsByInvoiceId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", invoiceId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Add(new InvoiceDetail
                            {
                                InvoiceDetailId = (int)reader["InvoiceDetailId"],
                                InvoiceId = (int)reader["InvoiceId"],
                                ProductId = (int)reader["ProductId"],
                                ProductName = reader["ProductName"].ToString(),
                                Quantity = (int)reader["Quantity"],
                                UnitPrice = (decimal)reader["UnitPrice"],                                
                            });
                        }
                    }
                }
            }
            return details;
        }

        public void AddInvoiceDetail(InvoiceDetail detail)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("AddInvoiceDetail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", detail.InvoiceId);
                    command.Parameters.AddWithValue("@ProductId", detail.ProductId);
                    command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateInvoiceDetail(InvoiceDetail detail)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdateInvoiceDetail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceDetailId", detail.InvoiceDetailId);
                    command.Parameters.AddWithValue("@Quantity", detail.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteInvoiceDetail(int detailId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DeleteInvoiceDetail", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceDetailId", detailId);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Invoice Payments: Retrieve payments by InvoiceId
        public IEnumerable<InvoicePayment> GetInvoicePaymentsByInvoiceId(int invoiceId)
        {
            var payments = new List<InvoicePayment>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetInvoicePaymentsByInvoiceId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", invoiceId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new InvoicePayment
                            {
                                PaymentId = (int)reader["PaymentId"],
                                InvoiceId = (int)reader["InvoiceId"],
                                PaymentDate = (DateTime)reader["PaymentDate"],
                                Amount = (decimal)reader["Amount"]
                            });
                        }
                    }
                }
            }
            return payments;
        }

        // Invoice Payments: Add a new payment
        public void AddInvoicePayment(InvoicePayment payment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("AddInvoicePayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@InvoiceId", payment.InvoiceId);
                    command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Invoice Payments: Update an existing payment
        public void UpdateInvoicePayment(InvoicePayment payment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdateInvoicePayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentId", payment.PaymentId);
                    command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Invoice Payments: Delete a payment
        public void DeleteInvoicePayment(int paymentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DeleteInvoicePayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentId", paymentId);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Invoice Payments: Calculate total payments for an invoice
        public decimal GetTotalPaymentsForInvoice(int invoiceId)
        {
            decimal totalPayments = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT SUM(Amount) FROM InvoicePayments WHERE InvoiceId = @InvoiceId", connection))
                {
                    command.Parameters.AddWithValue("@InvoiceId", invoiceId);
                    totalPayments = (decimal)(command.ExecuteScalar() ?? 0);
                }
            }
            return totalPayments;
        }

    }

}
