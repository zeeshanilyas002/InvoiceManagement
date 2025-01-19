using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    public IActionResult message(string message)
    {
        ViewData["ErrorMessage"] = message;
        return View("GlobalError");
    }
}
