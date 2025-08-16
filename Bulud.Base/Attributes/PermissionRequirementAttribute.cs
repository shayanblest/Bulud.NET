using Bulud.Base.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Bulud.Base.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PermissionRequirementAttribute(string permission) : AuthorizeAttribute, IAuthorizationFilter
{
    private string Permission { get; } = permission;
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        // Create the requirement
        var requirement = new PermissionRequirement(Permission);

        // Authorize the user against the requirement
        var result = authorizationService.AuthorizeAsync(context.HttpContext.User, null, requirement).Result;

        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();  // Deny access if authorization fails
        }
    }
}