using Dapper;
using FluentValidation;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MadiatrProject.Command;

public class UserRegisterCommand:IRequest<int>
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Token { get; set; }

    private class UserRegisterCommandHandeler : IRequestHandler<UserRegisterCommand, int>
    {
        private readonly MDBContext _dbcontext;
        
        public UserRegisterCommandHandeler(MDBContext dbcontext)
        {
                _dbcontext = dbcontext;
               
        }
        public async Task<int> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            int result = 0;
            bool usernameExists = await IsUsernameExistsAsync(request.UserName);

            if (usernameExists)
            {
                throw new InvalidOperationException("Username already exists.");
                
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = hashedPassword,
                Token = request.Token,
            };
            await _dbcontext.User.AddAsync(user);
            result = _dbcontext.SaveChanges();

            return result;
        }

        public async Task<bool> IsUsernameExistsAsync(string userName)
        {
            var connection = _dbcontext.GetSqlConnection();
            var checkUsernameQuery = "SELECT COUNT(*) FROM [User] WHERE UserName = @UserName";
            var usernameExists = await connection.QuerySingleOrDefaultAsync<int>(checkUsernameQuery, new { UserName = userName });
            return usernameExists > 0;
        }
    }
}
public class AuthenticationValidator : AbstractValidator<UserRegisterCommand>
{
    public AuthenticationValidator()
    {
        RuleFor(a => a.FirstName)
        .NotEmpty()
        .MaximumLength(10)
        .MinimumLength(3);
        RuleFor(a => a.LastName)
            .NotEmpty()
            .MaximumLength(10)
            .MinimumLength(3);

        RuleFor(a => a.UserName)
            .MaximumLength(10)
            .MinimumLength(3);
            

        RuleFor(a => a.Password)
           .Length(4,8)
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



