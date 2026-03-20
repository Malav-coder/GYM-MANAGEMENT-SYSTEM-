using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GymManagement.Filters;

public class MemberOnlyFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var session = context.HttpContext.Session;
        
        if (session.GetString("IsLoggedIn") != "true" || session.GetString("Role") != "Member")
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        
        await next();
    }
}
