using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    public IActionResult Index(string message)
    {
        ViewData["ErrorMessage"] = message;
        return View("GlobalError");
    }
}
