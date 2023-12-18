using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadiatrProject.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredPermissions;
    //public AuthorizeAttribute(string roles)
    //{
    //    Role = roles;
    //}
    public AuthorizeAttribute(string permission)
    {
        _requiredPermissions = permission.Split(','); ;
    }
    private readonly MDBContext _dbContext;
    public AuthorizeAttribute(MDBContext dbContext)
    {
            _dbContext = dbContext;
    }
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        string? token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        UserAuthenticationService? userService =  context.HttpContext.RequestServices.GetService(typeof(UserAuthenticationService)) as UserAuthenticationService;
        var user = userService.ValidateToken(token);
        int? userID = user;

        if (userID == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roles = context.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
        

  
       var permissionRoles = userService.findUser(userID);
    
        bool hasPermission = permissionRoles.RoleName.Any(role => roles.Contains(role.ToString()));

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }
        var Per = userService.HasAnyPermission(permissionRoles.RoleId,_requiredPermissions);
        if (!Per)
        {
            context.Result = new ForbidResult("You Don Have Permission");
            return;
        }

        Claim claim = new Claim(ClaimTypes.Name, userID.ToString() ?? "0");
        ClaimsIdentity? identity = new ClaimsIdentity(new[] { claim }, "BasicAuthentication");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        context.HttpContext.User = principal;
    }
}
