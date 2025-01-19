using InvoiceManagement.Data;
using InvoiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
[AuthenticationFilter]
public class InvoicesController : Controller
{
    private readonly IInvoicesRepository _invoicesRepository;
    private readonly IProductRepository _productRepository;

    public InvoicesController(IInvoicesRepository invoicesRepository, IProductRepository productRepository)
    {
        _invoicesRepository = invoicesRepository;
        _productRepository = productRepository;
    }

    // List invoices with search and pagination
    public IActionResult Index(string searchTerm, DateTime? searchDate, int page = 1)
    {
        var invoices = _invoicesRepository.GetAllInvoices();

        // Search and filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            invoices = invoices.Where(i => i.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        if (searchDate.HasValue)
        {
            invoices = invoices.Where(i => i.InvoiceDate.Date == searchDate.Value.Date);
        }

        // Pagination
        int pageSize = 10;
        var pagedInvoices = invoices.ToPagedList(page, pageSize);

        ViewData["SearchTerm"] = searchTerm;
        ViewData["SearchDate"] = searchDate;

        return View(pagedInvoices);
    }

    // Create a new invoice
    public IActionResult Create()
    {
        var invoice = new Invoice
        {
            InvoiceDetails = new List<InvoiceDetail>(),
            InvoicePayments = new List<InvoicePayment>()
        };

        ViewData["Products"] = _productRepository.GetAll();
        return View("Edit", invoice);
    }

    // Edit an existing invoice
    public IActionResult Edit(int id)
    {
        var invoice = _invoicesRepository.GetInvoiceById(id);
        if (invoice == null)
        {
            return NotFound();
        }

        invoice.InvoiceDetails = _invoicesRepository.GetInvoiceDetailsByInvoiceId(id).ToList();
        invoice.InvoicePayments = _invoicesRepository.GetInvoicePaymentsByInvoiceId(id).ToList();

        ViewData["Products"] = _productRepository.GetAll();
        return View(invoice);
    }

    // Save (Add or Update) an invoice
    [HttpPost]
    public IActionResult Save(Invoice invoice)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Products"] = _productRepository.GetAll();
            return View("Edit", invoice);
        }

        if (invoice.InvoiceId == 0)
        {
            // Add a new invoice
            var newInvoiceId = _invoicesRepository.AddInvoice(invoice);

            foreach (var detail in invoice.InvoiceDetails)
            {
                detail.InvoiceId = newInvoiceId;
                _invoicesRepository.AddInvoiceDetail(detail);
            }

            foreach (var payment in invoice.InvoicePayments)
            {
                payment.InvoiceId = newInvoiceId;
                _invoicesRepository.AddInvoicePayment(payment);
            }
        }
        else
        {
            // Update existing invoice
            _invoicesRepository.UpdateInvoice(invoice);

            foreach (var detail in invoice.InvoiceDetails)
            {
                if (detail.InvoiceDetailId == 0)
                {
                    detail.InvoiceId = invoice.InvoiceId;
                    _invoicesRepository.AddInvoiceDetail(detail);
                }
                else
                {
                    _invoicesRepository.UpdateInvoiceDetail(detail);
                }
            }

            foreach (var payment in invoice.InvoicePayments)
            {
                if (payment.PaymentId == 0)
                {
                    payment.InvoiceId = invoice.InvoiceId;
                    _invoicesRepository.AddInvoicePayment(payment);
                }
                else
                {
                    _invoicesRepository.UpdateInvoicePayment(payment);
                }
            }
        }

        return RedirectToAction("Index");
    }

    // Delete an invoice
    public IActionResult Delete(int id)
    {
        var invoice = _invoicesRepository.GetInvoiceById(id);
        if (invoice == null)
        {
            return NotFound();
        }
        return View(invoice);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _invoicesRepository.DeleteInvoice(id);
        return RedirectToAction("Index");
    }

    // Get invoice details dynamically
    public IActionResult GetInvoiceDetails(int id)
    {
        var details = _invoicesRepository.GetInvoiceDetailsByInvoiceId(id);
        return Json(details);
    }

    // Add a new line item
    [HttpPost]
    public IActionResult AddInvoiceDetail([FromBody] InvoiceDetail detail)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid line item data.");
        }

        _invoicesRepository.AddInvoiceDetail(detail);
        return Ok();
    }

    // Update an existing line item
    [HttpPost]
    public IActionResult UpdateInvoiceDetail([FromBody] InvoiceDetail detail)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid line item data.");
        }

        _invoicesRepository.UpdateInvoiceDetail(detail);
        return Ok();
    }

    // Delete a line item
    [HttpPost]
    public IActionResult DeleteInvoiceDetail(int detailId)
    {
        _invoicesRepository.DeleteInvoiceDetail(detailId);
        return Ok();
    }

    // Manage payments for an invoice
    public IActionResult Payments(int id)
    {
        var payments = _invoicesRepository.GetInvoicePaymentsByInvoiceId(id);
        var invoice = _invoicesRepository.GetInvoiceById(id);

        ViewData["InvoiceId"] = id;
        ViewData["TotalAmount"] = invoice?.TotalAmount ?? 0;

        return View(payments);
    }


    // Add a new payment
    [HttpPost]
    public IActionResult AddPayment([FromBody] InvoicePayment payment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid payment data.");
        }

        _invoicesRepository.AddInvoicePayment(payment);
        return Ok();
    }

    // Update an existing payment
    [HttpPost]
    public IActionResult UpdatePayment([FromBody] InvoicePayment payment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid payment data.");
        }

        _invoicesRepository.UpdateInvoicePayment(payment);
        return Ok();
    }

    // Delete a payment
    [HttpPost]
    public IActionResult DeletePayment(int paymentId)
    {
        _invoicesRepository.DeleteInvoicePayment(paymentId);
        return Ok();
    }
}
