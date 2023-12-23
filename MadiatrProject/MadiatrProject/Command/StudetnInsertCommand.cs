using Dapper;
using FluentValidation;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using MadiatrProject.Cache;
using MadiatrProject.Queries;

namespace MadiatrProject.Command;

public class StudetnIntCommand:IRequest<int>
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }
    public int RollNo { get; set; }

    private class StudetnInsertCommandHandler : IRequestHandler<StudetnIntCommand, int>
    {

        private readonly MDBContext _cotext;
        private readonly ICacheService _cacheService;

        public StudetnInsertCommandHandler(MDBContext context, ICacheService cacheService)
        {
            _cotext = context;
            _cacheService = cacheService;
        }


        public async Task<int> Handle(StudetnIntCommand request, CancellationToken cancellationToken)
        {
            string key = "Students";
            List<StudentsDto>? sdata = await _cacheService.GetAsync<List<StudentsDto>>(key, cancellationToken);
            if (sdata != null)
            {
               await _cacheService.RemoveAsync(key, cancellationToken);
            }
            var connection = _cotext.GetSqlConnection();

            var query = "Insert into Students (StudentName,StudentEmail,FatherName,MotherName,RollNo) Values(@StudentName,@StudentEmail,@FatherName,@MotherName,@RollNo)";
            var parameters = new DynamicParameters();

            parameters.Add("StudentName", request.StudentName);
            parameters.Add("StudentEmail", request.StudentEmail);
            parameters.Add("FatherName",request.FatherName);
            parameters.Add("MotherName", request.MotherName);
            parameters.Add("RollNo", request.RollNo);

            var studentInsert =await connection.ExecuteAsync(query, parameters);
            return studentInsert;
            
        }
    }
}

public class StudentValidator:AbstractValidator<StudetnIntCommand>
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
            .NotEmpty()
            .MaximumLength(10)
            .MinimumLength(3);
        RuleFor(a => a.RollNo)
            .NotEmpty()
            .InclusiveBetween(1,100)
            .WithMessage("{PropertyName} have to be between 1 and 100");
            
            

    }
}


