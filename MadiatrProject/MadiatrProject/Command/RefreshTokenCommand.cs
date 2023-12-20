using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static MadiatrProject.Command.RefreshTokenCommand;

namespace MadiatrProject.Command;

public class RefreshTokenCommand : IRequest<UserCredintial>
{
    public string Token { get; set; } = null!;
    private class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, UserCredintial>
    {
        private readonly UserAuthenticationService _authService;
        private readonly SDBContext _sdbContext;
        private readonly AppSettings _appSettings;



        public RefreshTokenCommandHandler(UserAuthenticationService authService, SDBContext sdbContext, IOptions<AppSettings> appSettings)
        {
            _authService = authService;
            _sdbContext = sdbContext;
            _appSettings = appSettings.Value;
        }


        public async Task<UserCredintial> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (request.Token == null)
            {

            }
           var response = _authService.TokenReader(request.Token);

            var userData = await _authService.FindUserDetails(response.UserId);
            Guid sessionId = Guid.Parse(response.sessionId);
            //bool result = ISsessionValidated(sessionID);
            //if (!result)
            //{
            //    throw new UnauthorizedAccessException($"{userData.UserName} Have to LogIn");
            //}
            //Guid sessionId =  Guid.NewGuid();
            var refreshToken = _authService.GenerateToken(sessionId, response.UserId, response.RoleId, response.permission, _appSettings.key);
           
            _authService.UpdateSessionWiseUserData(sessionId,  refreshToken);
            var userCredintial = new UserCredintial
            {
                RoleId = response.RoleId,
                UserId = response.UserId,
                UserName = userData.UserName,
                Token = refreshToken,
                RoleName = userData.RoleName,
            };

            return userCredintial;
        }

        private bool ISsessionValidated(Guid sessionId)
        {

            var allowedTimeDifference = TimeSpan.FromMinutes(5);

            var logInTime = _sdbContext.SessionTbl
                .Where(s => s.SessionId == sessionId)
                .Select(a => a.LogInTime)
                .FirstOrDefault();

            if (logInTime == null)
            {  
                return false;
            }

            var timeDifference = DateTime.Now - logInTime;
            if (timeDifference >= allowedTimeDifference)
            {
                return false; 
            }
            return true; 
        }
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


}
