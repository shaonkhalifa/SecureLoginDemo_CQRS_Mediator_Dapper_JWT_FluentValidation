using Dapper;
using FluentValidation;
using MadiatrProject.DbContexts;
using MediatR;

namespace MadiatrProject.Command;

public class StudentUpdateCommand : IRequest<int>
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public int RollNo { get; set; }
    public class StudentUpdateCommandHandelar : IRequestHandler<StudentUpdateCommand, int>
    {
        private readonly MDBContext _dbContext;
        public StudentUpdateCommandHandelar(MDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> Handle(StudentUpdateCommand request, CancellationToken cancellationToken)
        {
            var connection = _dbContext.GetSqlConnection();
            var query = @"EXEC UpdateStudentPrc @StudentId,@StudentName,@StudentEmail,@FatherName,@MotherName,@RollNo";
            var parameters = new DynamicParameters();
            parameters.Add("StudentId", request.StudentId);
            parameters.Add("StudentName", request.StudentName);
            parameters.Add("StudentEmail", request.StudentEmail);
            parameters.Add("FatherName", request.FatherName);
            parameters.Add("MotherName", request.MotherName);
            parameters.Add("RollNo", request.RollNo);

            var studentUpdate = await connection.ExecuteAsync(query, parameters);
            return studentUpdate;
        }
    }
}
    public class StudentUpdateValidator : AbstractValidator<StudentUpdateCommand>
    {
        public StudentUpdateValidator()
        {
            RuleFor(a => a.StudentName)
            .NotEmpty()
            .MaximumLength(10)
            .MinimumLength(3);
            RuleFor(a => a.StudentEmail)
                .NotEmpty()
                .EmailAddress();
            RuleFor(a => a.FatherName)
                .MaximumLength(10);
            RuleFor(a => a.MotherName)
                .NotEmpty()
                .MaximumLength(10)
                .MinimumLength(3);
            RuleFor(a => a.RollNo)
                .NotEmpty()
                .InclusiveBetween(1, 100)
                .WithMessage("{PropertyName} have to be between 1 and 100");



        }
    }
