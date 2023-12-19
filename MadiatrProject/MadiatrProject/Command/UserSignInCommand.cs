using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MadiatrProject.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MadiatrProject.Command;

public class UserSignInCommand : IRequest<UserCredintial>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    private class UserSignInCommandHandler : IRequestHandler<UserSignInCommand, UserCredintial>
    {

        private readonly MDBContext _dbcontext;
        private readonly IMediator _mediator;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;
        private readonly UserAuthenticationService _userAuthService;
        private readonly SDBContext _sdbContext;

        public UserSignInCommandHandler(MDBContext dbcontext, IMediator mediator, IOptions<AppSettings> appSettings, IConfiguration configuration, UserAuthenticationService userAuthService, SDBContext sdbContext)
        {
            _dbcontext = dbcontext;
            _mediator = mediator;
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _userAuthService = userAuthService;
            _sdbContext = sdbContext;
        }
        public async Task<UserCredintial> Handle(UserSignInCommand request, CancellationToken cancellationToken)
        {
            //var authenticUserQuery = new AuthenticUserQuery
            //{
            //    UserName = request.UserName,
            //    Password = request.Password
            //};
            // var credential = await _mediator.Send(authenticUserQuery);
            var credential = await _userAuthService.VerifyUser(request.UserName, request.Password);
            if (credential != null)
            {
                var sessionId = Guid.NewGuid();
                var roles = GetRolesForUser(credential.UserId);
                var permissionIds = await _dbcontext.PermissionAssign
                   .Where(a => a.RoleId == roles.RoleId)
                   .Select(a => a.PermissionId)
                   .ToListAsync();

                string joinedPermissionId = string.Join(",", permissionIds);

                var token = GenerateToken(sessionId, credential, roles, joinedPermissionId, _appSettings.key);
                credential.Token = token;
                credential.Password = "";
                //_userAuthService.
                //var validToken = ValidateToken(token);
                //if (validToken == null)
                //{
                //    throw new UnauthorizedAccessException("Invalid User");
                //}
                // Assuming this code is inside an asynchronous method
               

                // Now 'joinedPermissionId' contains a comma-separated string of permission IDs

                _userAuthService.InsertSessionWiseUserData(sessionId, credential.UserId, credential.Token, roles.RoleId, joinedPermissionId);
                var userCredintial = new UserCredintial
                {
                    RoleId = roles.RoleId,
                    UserId = credential.UserId,
                    UserName = credential.UserName,
                    Token = credential.Token,
                    RoleName = roles.RoleName,
                };

                return userCredintial;
            }


            throw new UnauthorizedAccessException("Invalid User");

        }
        

        public Role GetRolesForUser(int userId)
        {

            var role = _dbcontext.RoleAssain
                    .Where(ra => ra.UserId == userId)
                    .Join(
                        _dbcontext.Role,
                        ra => ra.RoleId,
                        role => role.RoleId,
                        (ra, role) => new Role
                        {
                            RoleId = role.RoleId,
                            RoleName = role.RoleName
                        }
                    ).FirstOrDefault();
            return role;
        }
        private string GenerateToken(Guid sessionId, User user, Role roles,string joinedPermissionId, string secret)
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
                     new Claim(ClaimTypes.Name, user.UserId.ToString()),
                     new Claim(ClaimTypes.Role,roles.RoleId.ToString()),
                     new Claim(ClaimTypes.Dns,joinedPermissionId),
                     new Claim(ClaimTypes.Dsa, sessionId.ToString())

                 }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

    }


}
public class UserCredintial
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? Token { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }

}
public class SessionDto
{
    public Guid SessionId { get; set; }
    public DateTime ExpireTime { get; set; }
    public DateTime LogInTime { get; set; }
    public string? Token { get; set; }
    public int? RoleID { get; set; }
    public int? UserID { get; set; }
    public string Permission { get; set; }
}