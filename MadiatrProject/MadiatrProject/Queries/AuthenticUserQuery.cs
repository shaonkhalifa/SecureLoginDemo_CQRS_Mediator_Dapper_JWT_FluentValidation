using Dapper;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;

namespace MadiatrProject.Queries;

public class AuthenticUserQuery:IRequest<User>
{
    //public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    private class AuthenticUserQueryHandler : IRequestHandler<AuthenticUserQuery, User>
    {
        public readonly MDBContext _dbContext;
        public AuthenticUserQueryHandler(MDBContext dbContext)
        {
                _dbContext = dbContext;
        }
        public async Task<User> Handle(AuthenticUserQuery request, CancellationToken cancellationToken)
        {

            var connection = _dbContext.GetSqlConnection();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var data = "SELECT * FROM [User] WHERE UserName = @UserName ";
            var queryResult = await connection.QuerySingleOrDefaultAsync<User>(data, new { UserName = request.UserName });//, Password = hashedPassword
            if (queryResult == null)
            {
                throw new InvalidOperationException("No user found.");
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, queryResult.Password))
            {
                throw new InvalidOperationException("Invalid password.");
            }
            return queryResult;
        }
    }
}


//public class UsercredetialDto
//{
//    public int UserId { get; set; }
//    public string FirstName { get; set; } = null!;
//    public string LastName { get; set; } = null!;
//    public string UserName { get; set; } = null!;
//    public string Password { get; set; } = null!;
//    public string? Token { get; set; }
//}
