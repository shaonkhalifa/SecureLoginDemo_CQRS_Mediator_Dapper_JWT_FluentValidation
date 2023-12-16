using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MediatR;
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

        public UserSignInCommandHandler(MDBContext dbcontext, IMediator mediator, IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _mediator = mediator;
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }
        public async Task<string> Handle(UserSignInCommand request, CancellationToken cancellationToken)
        {
            var authenticUserQuery = new AuthenticUserQuery
            {
                UserName = request.UserName,
                Password = request.Password
            };
            var credential = await _mediator.Send(authenticUserQuery);

            if (credential != null)
            {
                var token = GenerateToken(credential, _appSettings.key);
                credential.Token = token;
                credential.Password = "";
                return token;
            }

            
            throw new UnauthorizedAccessException("Invalid User");

        }
        private string GenerateToken(User user, string secret)
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
            
        }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

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
    }
    
   
}
public class UserCredintial
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

}