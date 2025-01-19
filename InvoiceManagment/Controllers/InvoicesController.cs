using InvoiceManagement.Data;
using InvoiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
[AuthenticationFilter]
public class InvoicesController : Controller
{
    private readonly IInvoicesRepository _invoicesRepository;
    private readonly IProductRepository _productRepository;

    public InvoicesController(IInvoicesRepository invoiceRepository, IProductRepository productRepository)
    {
        _invoicesRepository = invoiceRepository;
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

        // Convert the product list into SelectListItem
        ViewData["Products"] = _productRepository.GetAll()
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

        return View("Edit", invoice);
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

        // Convert the product list into SelectListItem
        ViewData["Products"] = _productRepository.GetAll()
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

        return View(invoice);
    }
    public IActionResult CreateOrEditInvoice(int? id)
    {
        var model = new Invoice();

        if (id.HasValue)
        {
            model = _invoicesRepository.GetInvoiceById(id.Value);
            if (model == null)
            {
                return NotFound();
            }
        }

        // Fetch product list from the repository
        ViewBag.Products = _productRepository.GetAll()
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

        return View(model);
    }

    [HttpPost]
    public IActionResult CreateOrEditInvoice(Invoice model)
    {
       
        if (model.InvoiceId == 0)
        {
            InvoicePayment InvoicePayment =new InvoicePayment();
            InvoicePayment.Amount = model.TotalAmount;
            InvoicePayment.PaymentDate = model.InvoiceDate;
            model.InvoicePayments.Add(InvoicePayment);
           _invoicesRepository.AddInvoiceWithDetailsAndPayments(model);
        }
        else
        {
            _invoicesRepository.UpdateInvoiceWithDetailsAndPayments(model);
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

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var invoice = _invoicesRepository.GetInvoiceById(id);
        if (invoice == null)
        {
            return NotFound();
        }

        _invoicesRepository.DeleteInvoiceWithDetails(id);
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
    [HttpGet]
    public IActionResult Payments(int id)
    {
        // Get payments for the specified invoice ID
        var payments = _invoicesRepository.GetAllPayments();
        var invoice = _invoicesRepository.GetInvoiceById(id);

        // Pass invoice details to the view
        ViewData["InvoiceId"] = id;
        ViewData["TotalAmount"] = invoice?.TotalAmount ?? 0;

        return View(payments);
    }

    [HttpPost]
    public IActionResult SearchPayments(int id, string searchQuery)
    {
        // Get payments for the specified invoice ID
        var payments = _invoicesRepository.GetAllPayments(id); 

        // Filter payments based on the search query
        if (!string.IsNullOrEmpty(searchQuery))
        {
            payments = payments
                .Where(p => p.PaymentId.ToString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            p.PaymentDate.ToString("yyyy-MM-dd").Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                            p.Amount.ToString("C").Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return PartialView("_PaymentsGrid", payments);
    }

    // Manage payments for an invoice
    

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
