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
    public IActionResult Index()
    {
        var invoices = _invoicesRepository.GetAllInvoices();
        return View(invoices);
    }



    // Create a new invoice
    // GET: Create a new invoice
    public IActionResult Create()
    {
        var invoice = new Invoice
        {
            InvoiceDetails = new List<InvoiceDetail>(),
            InvoicePayments = new List<InvoicePayment>()
        };

        ViewData["Products"] = _productRepository.GetAll(); // Ensure this returns a valid list of products
        return View(invoice);
    }


    // POST: Save (Add or Update) an invoice
    [HttpPost]
    public IActionResult Save(Invoice invoice)
    {
        if (!ModelState.IsValid)
        {
            // Reload products if validation fails
            ViewData["Products"] = _productRepository.GetAll();
            return View("Edit", invoice);
        }

        if (invoice.InvoiceId == 0)
        {
            // Add a new invoice
            var newInvoiceId = _invoicesRepository.AddInvoice(invoice);

            // Save invoice details
            foreach (var detail in invoice.InvoiceDetails)
            {
                detail.InvoiceId = newInvoiceId;
                _invoicesRepository.AddInvoiceDetail(detail);
            }

            // Save invoice payments
            foreach (var payment in invoice.InvoicePayments)
            {
                payment.InvoiceId = newInvoiceId;
                _invoicesRepository.AddInvoicePayment(payment);
            }
        }
        else
        {
            // Update an existing invoice
            _invoicesRepository.UpdateInvoice(invoice);

            // Update or add invoice details
            foreach (var detail in invoice.InvoiceDetails)
            {
                if (detail.InvoiceDetailId == 0)
                {
                    // Add new detail
                    detail.InvoiceId = invoice.InvoiceId;
                    _invoicesRepository.AddInvoiceDetail(detail);
                }
                else
                {
                    // Update existing detail
                    _invoicesRepository.UpdateInvoiceDetail(detail);
                }
            }

            // Update or add invoice payments
            foreach (var payment in invoice.InvoicePayments)
            {
                if (payment.PaymentId == 0)
                {
                    // Add new payment
                    payment.InvoiceId = invoice.InvoiceId;
                    _invoicesRepository.AddInvoicePayment(payment);
                }
                else
                {
                    // Update existing payment
                    _invoicesRepository.UpdateInvoicePayment(payment);
                }
            }
        }

        return RedirectToAction("Index");
    }

    // GET: Edit an existing invoice
    public IActionResult Edit(int id)
    {
        var invoice = _invoicesRepository.GetInvoiceById(id);
        if (invoice == null)
        {
            return NotFound();
        }

        // Load invoice details and payments
        invoice.InvoiceDetails = _invoicesRepository.GetInvoiceDetailsByInvoiceId(id).ToList();
        invoice.InvoicePayments = _invoicesRepository.GetInvoicePaymentsByInvoiceId(id).ToList();

        // Load products for the dropdown
        ViewData["Products"] = _productRepository.GetAll();

        return View(invoice);
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
