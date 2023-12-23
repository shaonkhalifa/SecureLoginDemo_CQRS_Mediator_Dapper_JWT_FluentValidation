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

                var token =_userAuthService.GenerateToken(sessionId, credential.UserId, roles.RoleId, joinedPermissionId, _appSettings.key);
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
                    RefereshToken=sessionId.ToString()
                    
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
       

    }


}
public class UserCredintial
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? Token { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RefereshToken { get; set;}

}
