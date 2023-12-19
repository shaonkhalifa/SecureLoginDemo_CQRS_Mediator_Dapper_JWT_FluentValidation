using MadiatrProject.DbContexts;
using MadiatrProject.Enums;
using MadiatrProject.Model;
using MadiatrProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace MadiatrProject.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{  private PermissionEnum _requiredPermissions;
    //private readonly string[] _requiredPermissions;
    //public AuthorizeAttribute(string roles)
    //{
    //    Role = roles;
    //}
    public AuthorizeAttribute(PermissionEnum permission)
    {
        //_requiredPermissions = permission.Split(','); ;
        _requiredPermissions = permission;
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


        bool hasPermission = permissionRoles.RoleId.ToString().Equals(roles);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }
        //var RoleId = permissionRoles.Select(role => role.RoleId).ToList();
        int permissionID = (int)_requiredPermissions;
        var Per = userService.HasAnyPermission(permissionRoles.RoleId, permissionID);
        if (!Per)
        {
            context.Result = new ForbidResult("You Don't Have Permission");
            return;
        }

        Claim claim = new Claim(ClaimTypes.Name, userID.ToString() ?? "0");
        ClaimsIdentity? identity = new ClaimsIdentity(new[] { claim }, "BasicAuthentication");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        context.HttpContext.User = principal;
    }
}
