using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebHike.Data;

namespace WebHike.Security;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var email = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["WebHike:AdminEmail"];
        var userId = context.HttpContext.Session.GetInt32("UserId");

        if (string.IsNullOrWhiteSpace(email) || userId is null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<HikeDbContext>();
        var user = db.Users.SingleOrDefault(x => x.Id == userId.Value);

        if (user is null || !string.Equals(user.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))
            context.Result = new RedirectToActionResult("Index", "Main", new { area = "" });
    }
}
