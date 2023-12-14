using Dapper;
using FluentValidation;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MadiatrProject.Command;

public class StudetnInsertCommand:IRequest<int>
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public int RollNo { get; set; }

    private class StudetnInsertCommandHandler : IRequestHandler<StudetnInsertCommand, int>
    {
        private readonly IDbConnection _dbConnection;
        private readonly MDBContext _cotext;
        private readonly StudentValidator _validations;
        public StudetnInsertCommandHandler(MDBContext context, IDbConnection dbConnection, StudentValidator validations)
        {
            _cotext = context;
            _dbConnection = dbConnection;
            _validations = validations; 
        }


        public async Task<int> Handle(StudetnInsertCommand request, CancellationToken cancellationToken)
        {
            

            var query = "Insert into Students (StudentName,StudentEmail,FatherName,MotherName,RollNo) Values(@StudentName,@StudentEmail,@FatherName,@MotherName,@RollNo)";
            var parameters = new DynamicParameters();

            parameters.Add("StudentName", request.StudentName);
            parameters.Add("StudentEmail", request.StudentEmail);
            parameters.Add("FatherName",request.FatherName);
            parameters.Add("MotherName", request.MotherName);
            parameters.Add("RollNo", request.RollNo);

            var studentInsert =await _dbConnection.ExecuteAsync(query, parameters);
            return studentInsert;
            
        }
    }
}

public class StudentValidator:AbstractValidator<Students>
{
    public StudentValidator()
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
            .Null()
            .MaximumLength(10)
            .MinimumLength(3);
        RuleFor(a => a.RollNo)
            .NotEmpty()
            .InclusiveBetween(1,100)
            .WithMessage("{PropertyName} have to be between 1 and 100");
            
            

    }
}


