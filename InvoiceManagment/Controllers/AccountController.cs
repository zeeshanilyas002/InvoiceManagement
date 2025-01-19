using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private const string HardcodedUsername = "admin";
    private const string HardcodedPassword = "password123";

    // GET: Login Page
    public IActionResult Login()
    {
        return View();
    }

    // POST: Handle Login
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (username == HardcodedUsername && password == HardcodedPassword)
        {
            // Generate a session ID and store it
            var sessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("SessionId", sessionId);

            // Redirect to home page or dashboard
            return RedirectToAction("Index", "Home");
        }

        // Invalid credentials
        ViewBag.Error = "Invalid username or password.";
        return View();
    }

    // Logout: Clear session and redirect to login page
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
