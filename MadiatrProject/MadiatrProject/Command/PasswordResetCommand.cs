using Dapper;
using FluentValidation;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MadiatrProject.Command;

public class PasswordResetCommand:IRequest<int>
{
    public int UserId { get; set; }
    public string? Password { get; set; }
    public string? NewPassword { get; set; }
    private class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, int>
    {
        private readonly MDBContext _dBContext;
        private readonly UserAuthenticationService _authService;

        public PasswordResetCommandHandler(MDBContext dBContext,UserAuthenticationService authService)
        {
            _dBContext = dBContext;
            _authService = authService;
        }

        public async Task<int> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
        {
            int result = 0;
           bool verificationResult= VerifyPassword(request.UserId,request.Password);
            if (!verificationResult)
            {
                throw new InvalidOperationException("Invalid password.");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var user= _dBContext.User.Where(a=>a.UserId == request.UserId).FirstOrDefault();
            if (user == null)
            {
                throw new InvalidOperationException("Invalid User.");
            }
            user.Password = hashedPassword;
            _dBContext.User.Update(user);
            result =await _dBContext.SaveChangesAsync();
            return result;
        }
        private bool VerifyPassword(int UserID,string? Password)
        {
            var connection = _dBContext.GetSqlConnection();
            var data = @"SELECT * FROM [User] WHERE UserId = @UserID ";
            var queryResult =  connection.QueryFirstOrDefault<User>(data, new { UserId = UserID });

            if (!BCrypt.Net.BCrypt.Verify(Password, queryResult.Password))
            {
                throw new InvalidOperationException("Invalid password.");
            }
            return true;
        }
    }
}
public class PasswordValidator : AbstractValidator<PasswordResetCommand>
{
    public PasswordValidator()
    {
        RuleFor(a => a.Password)
           .Length(4, 8)
           .Must(HasValidPassword)
           .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special symbol.");
    }

    private bool HasValidPassword(string password)
    {
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");

        return (lowercase.IsMatch(password) && uppercase.IsMatch(password) && digit.IsMatch(password) && symbol.IsMatch(password));
    }
}
public class UserDetail
{
    public string? UserName { get; set; }
}
public class UpdatePasswordDto
{
    public string? Password { get; set; }
}
