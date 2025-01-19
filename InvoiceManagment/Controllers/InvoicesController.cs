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

        ViewBag.Message = TempData["message"];
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View("Edit", invoice);
    }

    // POST: Save (Add or Update) an invoice  
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
    [HttpPost]
    public IActionResult CreateOrEditInvoice(Invoice model)
    {       
        // Check if InvoiceDetails is empty
        if (model.InvoiceDetails == null || !model.InvoiceDetails.Any() || string.IsNullOrEmpty(model.CustomerName))
        {
            TempData["message"] = "Please fill all necessary details!";
            return RedirectToAction("Create");
        }

        // Check for invalid amounts
        if (model.InvoiceDetails.Any(d => d.UnitPrice <= 0 || d.Quantity <= 0))
        {
            TempData["message"] = "All items must have a positive quantity and unit price.";
            return RedirectToAction("Create");
        }

        var payment = new InvoicePayment
        {
            Amount = model.TotalAmount,
            PaymentDate = model.InvoiceDate
        };
        model.InvoicePayments.Add(payment);

        if (model.InvoiceId == 0)            
            _invoicesRepository.AddInvoiceWithDetailsAndPayments(model);       
        else       
            _invoicesRepository.UpdateInvoiceWithDetailsAndPayments(model);
        

        TempData["SuccessMessage"] = "Invoice saved successfully!";
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
}
