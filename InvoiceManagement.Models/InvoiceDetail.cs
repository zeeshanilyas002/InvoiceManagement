using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Models
{
    public class InvoiceDetail
    {
        public int InvoiceDetailId { get; set; } // Primary Key
        public int InvoiceId { get; set; } // Foreign Key to Invoice
        public int ProductId { get; set; } // Foreign Key to Product

        public string ProductName { get; set; } // Name of the product
        public int Quantity { get; set; } // Quantity of the product
        public decimal UnitPrice { get; set; } // Price per unit of the product

        // Computed total for the line item (Quantity * UnitPrice)
        public decimal TotalPrice
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }
    }

}
