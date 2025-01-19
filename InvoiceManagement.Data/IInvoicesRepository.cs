using InvoiceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Data
{
    public interface IInvoicesRepository
    {
        // Invoice CRUD
        IEnumerable<Invoice> GetAllInvoices();
        Invoice GetInvoiceById(int id);
        int AddInvoice(Invoice invoice); // Returns the generated InvoiceId
        void UpdateInvoice(Invoice invoice);
        void DeleteInvoice(int id);

        // Invoice Details (Master-Detail)
        IEnumerable<InvoiceDetail> GetInvoiceDetailsByInvoiceId(int invoiceId);
        void AddInvoiceDetail(InvoiceDetail detail);
        void UpdateInvoiceDetail(InvoiceDetail detail);
        void DeleteInvoiceDetail(int detailId);

        // Invoice Payments
        IEnumerable<InvoicePayment> GetInvoicePaymentsByInvoiceId(int invoiceId);
        void AddInvoicePayment(InvoicePayment payment);
        void UpdateInvoicePayment(InvoicePayment payment);
        void DeleteInvoicePayment(int paymentId);

        // Calculations
        decimal GetTotalPaymentsForInvoice(int invoiceId);
    }

}
