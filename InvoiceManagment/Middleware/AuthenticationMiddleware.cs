using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthenticationFilter : Attribute, IAuthorizationFilter
{
    private const string SessionKey = "SessionId";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the session contains a valid SessionId
        var sessionId = context.HttpContext.Session.GetString(SessionKey);

        if (string.IsNullOrEmpty(sessionId))
        {
            // If not logged in, redirect to Login page
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
    }
}
