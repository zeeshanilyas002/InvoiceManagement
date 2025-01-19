using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Models
{
    public class InvoicePayment
    {
        public int PaymentId { get; set; } // Primary Key
        public int InvoiceId { get; set; } // Foreign Key to Invoice
        public DateTime PaymentDate { get; set; } // Date of payment
        public decimal Amount { get; set; } // Payment amount
    }

}
