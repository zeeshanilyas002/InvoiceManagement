using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }  // Primary Key
        [Required]
        public string CustomerName { get; set; } // Name of the customer
        [Required]
        public DateTime InvoiceDate { get; set; } // Date of the invoice

        // Navigation property for related invoice details
        public List<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

        // Navigation property for related payments
        public List<InvoicePayment> InvoicePayments { get; set; } = new List<InvoicePayment>();
        // Calculated total for the invoice (sum of all line items' TotalPrice)
        public decimal TotalInvoiceAmount { get; set; }

        public decimal TotalAmount
        {
            get
            {
                // Calculate the sum of invoice details; if zero, fallback to TotalInvoiceAmount
                var calculatedTotal = InvoiceDetails?.Sum(d => d.TotalPrice) ?? 0;
                return calculatedTotal > 0 ? calculatedTotal : TotalInvoiceAmount;
            }
        }


        // Calculated total payments made
        public decimal TotalPayments
        {
            get
            {
                return InvoicePayments?.Sum(p => p.Amount) ?? 0;
            }
        }

        // Remaining balance to be paid
        public decimal RemainingBalance
        {
            get
            {
                return TotalAmount - TotalPayments;
            }
        }
    }


}
