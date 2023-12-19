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
    private readonly SDBContext _sdbContext;
    private readonly ILogger<UserAuthenticationService> _logger;
    public UserAuthenticationService(MDBContext dbContext, IOptions<AppSettings> appSettings, IConfiguration configuration, SDBContext sdbContext, ILogger<UserAuthenticationService> logger)
    {

        _dbContext = dbContext;
        _appSettings = appSettings.Value;
        _configuration = configuration;
        _sdbContext = sdbContext;
        _logger = logger;
    }

    public async Task<User> VerifyUser(string UserName, string Password)
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
    public ClaimResponseDto ValidateToken(string token)
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
            var rolIdClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
            var permissionClaim = claimsPrincipal.FindFirst(ClaimTypes.Dns);
            var sessionIdClaim = claimsPrincipal.FindFirst(ClaimTypes.Dsa);
           

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) && rolIdClaim != null && int.TryParse(rolIdClaim.Value, out int roleId))
            {
                var ClaimResponse = new ClaimResponseDto
                {
                    UserId = userId,
                    RoleId = roleId,
                    permission = permissionClaim?.Value,
                    sessionId = sessionIdClaim?.Value
                };
                return ClaimResponse;
            }
        }
        catch
        {
            // Ignore validation errors and return null
        }

        return null;
    }
    public class ClaimResponseDto
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string? permission { get; set; }
        public string? sessionId { get; set; }

    }
    public RoleDto findUser(int? UserId)
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
                .Select(a => new RoleDto
                {
                    RoleId = a.RoleId,
                    RoleName = a.RoleName
                })
                .FirstOrDefault();

        //List<int> PermissionRoleID = _dbContext.RoleAssain
        //    .Where(a => a.UserId == UserId)
        //    .Select(a => a.RoleId)
        //    .ToList();



        //List<RoleDto> permissionRole = new List<RoleDto>();

        //foreach (var item in PermissionRoleID)
        //{
        //    var permissionRoles = _dbContext.Role
        //        .Where(a => a.RoleId == item)
        //        .Select(a => new RoleDto
        //        {
        //            RoleId = a.RoleId,
        //            RoleName = a.RoleName,
        //        })
        //        .FirstOrDefault();

        //    if (permissionRoles != null)
        //    {
        //        permissionRole.Add(permissionRoles);
        //    }
        //}

        return permissionRole;
    }

    //public List<int> RolePermissionList(int? RoleId)
    //{

    //}
    public async void InsertSessionWiseUserData(Guid SessionId, int? userId, string? token, int? roleId, string? permissionIds)
    {
        try
        {
            DateTime loginTime = DateTime.Now;
            DateTime expireTime = loginTime.AddHours(24);
            SessionTbl Session = new SessionTbl
            {
                SessionId = SessionId,
                LogInTime = loginTime,
                ExpireTime = expireTime,
                Permission = permissionIds,
                RoleID = roleId,
                Token = token,
                UserID = userId
            };
            await _sdbContext.SessionTbl.AddAsync(Session);
            _sdbContext.SaveChanges();


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting session data");
            throw;
        }


    }
    public bool HasAnyPermission(int roleId, int permissionNames)
    {
        var permissionNamesList = _dbContext.PermissionAssign
                   .Where(a => a.RoleId == roleId)
                   .Select(x => x.PermissionId)
                   .ToList();

        return permissionNamesList.Contains(permissionNames);

        //    var permissionNamesList = _dbContext.PermissionAssign
        //            .Where(a => a.RoleId == roleId)
        //      .Join(_dbContext.Permission,
        //      assign => assign.PermissionId,
        //      permission => permission.PermissionId,
        //      (assign, permission) => new { PermissionName = permission.PermissionId })
        //     .Select(x => x.PermissionName)
        //     .ToList();

        //return permissionNamesList.Any(permissionName => permissionNames.Contains(permissionName));

        //List<string> permissionList = new List<string>();
        //foreach (var roleId in roleIds)
        //{
        //    var permissionNamesList = _dbContext.PermissionAssign
        //            .Where(a => a.RoleId == roleId)
        //      .Join(_dbContext.Permission,
        //      assign => assign.PermissionId,
        //      permission => permission.PermissionId,
        //      (assign, permission) => new { PermissionName = permission.PermissionName })
        //     .Select(x => x.PermissionName)
        //     .ToList();
        //    permissionList.AddRange(permissionNamesList);
        //}


        //return permissionList.Any(permissionName => permissionNames.Contains(permissionName));
    }
}
public class RoleDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}
public class SessionDto
{
    public Guid SessionId { get; set; }
    public DateTime? ExpireTime { get; set; }
    public DateTime? LogInTime { get; set; }
    public string? Token { get; set; }
    public int? RoleID { get; set; }
    public int? UserID { get; set; }
    public string? Permission { get; set; }
}

