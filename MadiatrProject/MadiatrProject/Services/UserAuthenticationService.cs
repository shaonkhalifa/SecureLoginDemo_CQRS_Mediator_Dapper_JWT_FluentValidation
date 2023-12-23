using Azure;
using Azure.Core;
using Dapper;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static MadiatrProject.Command.RefreshTokenCommand;

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

    public async Task<User> VerifyUser(string userName, string password)
    {
        var connection = _dbContext.GetSqlConnection();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var data = "SELECT * FROM [User] WHERE UserName = @UserName ";
        var queryResult = await connection.QueryFirstOrDefaultAsync<User>(data, new { UserName = userName });
        if (queryResult == null)
        {
            throw new InvalidOperationException("No user found.");
        }
        if (!BCrypt.Net.BCrypt.Verify(password, queryResult.Password))
        {
            throw new InvalidOperationException("Invalid password.");
        }
        return queryResult;
    }
    public string GenerateToken(Guid sessionId, int userId, int roleId, string joinedPermissionId, string secret)
    {
        var secretKey = secret;
        if (Encoding.UTF8.GetBytes(secretKey).Length < 32)
        {
            throw new InvalidOperationException("Secret key should be at least 256 bits (32 bytes) long.");
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));



        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                     new Claim(ClaimTypes.Name, userId.ToString()),
                     new Claim(ClaimTypes.Role,roleId.ToString()),
                     new Claim(ClaimTypes.Dns,joinedPermissionId),
                     new Claim(ClaimTypes.Dsa, sessionId.ToString())

                 }),
            Expires = DateTime.Now.AddMinutes(5),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);

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
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.Name);
            var rolIdClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
            var permissionClaim = claimsPrincipal.FindFirst(ClaimTypes.Dns);
            var sessionIdClaim = claimsPrincipal.FindFirst(ClaimTypes.Dsa);
            Guid sessionID = Guid.Parse(sessionIdClaim.Value); ;
            bool result = IsTokenExpired(sessionID);
            if (!result)
            {
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
            else
            {
                return null;
            }


        }
        catch
        {
            return null;
        }

        return null;
    }
    private bool IsTokenExpired(Guid sessionId)
    {
        var expireTime = _sdbContext.SessionTbl
        .Where(s => s.SessionId == sessionId).Select(a => a.ExpireTime)
        .FirstOrDefault();

        if (expireTime != null || expireTime > DateTime.Now)
        {
            return false;
        }

        return true;
    }

    public async Task<UserDetailsDto> FindUserDetails(int userId)
    {
        if (userId == 0)
        {
            return null;
        }
        var userData = await (from u in _dbContext.User
                              join a in _dbContext.RoleAssain on u.UserId equals a.UserId
                              join r in _dbContext.Role on a.RoleId equals r.RoleId
                              where u.UserId == userId
                              select new UserDetailsDto
                              {
                                  UserName = u.UserName ?? "",
                                  RoleName = r.RoleName ?? ""
                              }).FirstOrDefaultAsync();

        return userData;
    }


    public async void InsertSessionWiseUserData(Guid sessionId, int userId, string? token, int roleId, string? permissionIds)
    {
        try
        {


            DateTime loginTime = DateTime.Now;
            DateTime expireTime = loginTime.AddMinutes(5);
            SessionTbl Session = new SessionTbl
            {
                SessionId = sessionId,
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
    public async void UpdateSessionWiseUserData(Guid sessionId, string? token)
    {
        var data = _sdbContext.SessionTbl.FirstOrDefault(a => a.SessionId == sessionId);
        if (data != null)
        {
            data.Token = token;
            data.ExpireTime = data.ExpireTime.AddMinutes(5);
            _sdbContext.SessionTbl.Update(data);
            await _sdbContext.SaveChangesAsync();
        }
    }

    public ClaimResponseDto TokenReader(string token)
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
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
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
            return null;
        }

        return null;
    }
    //public bool HasAnyPermission(int roleId, int permissionNames)
    //{
    //    var permissionNamesList = _dbContext.PermissionAssign
    //               .Where(a => a.RoleId == roleId)
    //               .Select(x => x.PermissionId)
    //               .ToList();

    //    return permissionNamesList.Contains(permissionNames);

    //    //    var permissionNamesList = _dbContext.PermissionAssign
    //    //            .Where(a => a.RoleId == roleId)
    //    //      .Join(_dbContext.Permission,
    //    //      assign => assign.PermissionId,
    //    //      permission => permission.PermissionId,
    //    //      (assign, permission) => new { PermissionName = permission.PermissionId })
    //    //     .Select(x => x.PermissionName)
    //    //     .ToList();

    //    //return permissionNamesList.Any(permissionName => permissionNames.Contains(permissionName));

    //    //List<string> permissionList = new List<string>();
    //    //foreach (var roleId in roleIds)
    //    //{
    //    //    var permissionNamesList = _dbContext.PermissionAssign
    //    //            .Where(a => a.RoleId == roleId)
    //    //      .Join(_dbContext.Permission,
    //    //      assign => assign.PermissionId,
    //    //      permission => permission.PermissionId,
    //    //      (assign, permission) => new { PermissionName = permission.PermissionName })
    //    //     .Select(x => x.PermissionName)
    //    //     .ToList();
    //    //    permissionList.AddRange(permissionNamesList);
    //    //}


    //    //return permissionList.Any(permissionName => permissionNames.Contains(permissionName));
    //}
}
public class UserDetailsDto
{
    public string? UserName { get; set; }
    public string? RoleName { get; set; }
}
public class ClaimResponseDto
{
    public int RoleId { get; set; }
    public int UserId { get; set; }
    public string? permission { get; set; }
    public string? sessionId { get; set; }


}

