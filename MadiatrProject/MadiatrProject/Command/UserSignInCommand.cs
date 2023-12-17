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

public class UserSignInCommand : IRequest<string>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    private class UserSignInCommandHandler : IRequestHandler<UserSignInCommand, string>
    {

        private readonly MDBContext _dbcontext;
        private readonly IMediator _mediator;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;
        private readonly UserAuthenticationService _userAuthService;

        public UserSignInCommandHandler(MDBContext dbcontext, IMediator mediator, IOptions<AppSettings> appSettings, IConfiguration configuration, UserAuthenticationService userAuthService)
        {
            _dbcontext = dbcontext;
            _mediator = mediator;
            _appSettings = appSettings.Value;
            _configuration = configuration;
            _userAuthService = userAuthService;
        }
        public async Task<string> Handle(UserSignInCommand request, CancellationToken cancellationToken)
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
                var token = GenerateToken(credential, _appSettings.key);
                credential.Token = token;
                credential.Password = "";
                //var validToken = ValidateToken(token);
                //if (validToken == null)
                //{
                //    throw new UnauthorizedAccessException("Invalid User");
                //}
                return token;
            }


            throw new UnauthorizedAccessException("Invalid User");

        }

        public string GetRolesForUser(int userId)
        {

            var role = _dbcontext.RoleAssain
       .Where(ra => ra.UserId == userId)
       .Join(
           _dbcontext.Role,
           ra => ra.RoleId,
           role => role.RoleId,
           (ra, role) => role.RoleName
       )
       .FirstOrDefault();
            return role;
        }
        private string GenerateToken(User user, string secret)
        {
            var secretKey = secret;
            if (Encoding.UTF8.GetBytes(secretKey).Length < 32)
            {
                throw new InvalidOperationException("Secret key should be at least 256 bits (32 bytes) long.");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var roles = GetRolesForUser(user.UserId);

    //        var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.Name, user.UserName),
    //};

    //        foreach (var role in roles)
    //        {
    //            claims.Add(new Claim(ClaimTypes.Role, role));
    //        }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserId.ToString()),
            new Claim(ClaimTypes.Role,roles.ToString())

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
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

}