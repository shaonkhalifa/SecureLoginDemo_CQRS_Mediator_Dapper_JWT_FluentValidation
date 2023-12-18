using Azure.Core;
using Dapper;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MadiatrProject.Services;

public class UserAuthenticationService
{

    private readonly MDBContext _dbContext;
    private readonly AppSettings _appSettings;
    private readonly IConfiguration _configuration;
    public UserAuthenticationService(MDBContext dbContext,IOptions<AppSettings> appSettings, IConfiguration configuration)
    {

        _dbContext = dbContext;
        _appSettings = appSettings.Value;
        _configuration = configuration;

    }

    public async Task<User> VerifyUser(string UserName,string Password)
    {
        var connection = _dbContext.GetSqlConnection();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
        var data = "SELECT * FROM [User] WHERE UserName = @UserName ";//AND Password = @Password
        var queryResult = await connection.QueryFirstOrDefaultAsync<User>(data, new { UserName = UserName });//, Password = hashedPassword
        if (queryResult == null)
        {
            throw new InvalidOperationException("No user found.");
        }
        if (!BCrypt.Net.BCrypt.Verify(Password, queryResult.Password))
        {
            throw new InvalidOperationException("Invalid password.");
        }
        return queryResult;
    }
    public int? ValidateToken(string token)
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.Name);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
        }
        catch
        {
            // Ignore validation errors and return null
        }

        return null;
    }
    public RoleDto  findUser(int? UserId)
    {
        if (UserId == 0)
        {
            return null;
        }

        int PermissionRoleID = _dbContext.RoleAssain
            .Where(a => a.UserId == UserId)
            .Select(a => a.RoleId)
            .FirstOrDefault();
        

        var permissionRole = _dbContext.Role
                .Where(a => a.RoleId == PermissionRoleID)
                .Select(a=> new RoleDto
                {
                    RoleId=a.RoleId,
                    RoleName=a.RoleName
                })
                .FirstOrDefault();

        //List<int> PermissionRoleID = _dbContext.RoleAssain
        //    .Where(a => a.UserId == UserId)
        //    .Select(a => a.RoleId)
        //    .ToList();

        //List<string> permissionRoles = new List<string>();

        //foreach (var item in PermissionRoleID)
        //{
        //    var permissionRole = _dbContext.Role
        //        .Where(a => a.RoleId == item)
        //        .Select(a => a.RoleName)
        //        .FirstOrDefault();

        //    if (permissionRole != null)
        //    {
        //        permissionRoles.Add(permissionRole);
        //    }
        //}

        return permissionRole;
    }

    //public List<int> RolePermissionList(int? RoleId)
    //{

    //}
   public bool HasAnyPermission(int roleId, IEnumerable<string> permissionNames)
    {
        var permissionNamesList = _dbContext.PermissionAssign
                     .Where(a => a.RoleId == roleId)
       .Join(_dbContext.Permission,
         assign => assign.PermissionId,
         permission => permission.PermissionId,
         (assign, permission) => new { PermissionName = permission.PermissionName })
       .Select(x => x.PermissionName)
    .ToList();

        return permissionNamesList.Any(permissionName => permissionNames.Contains(permissionName));
    }
}
public class RoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}
