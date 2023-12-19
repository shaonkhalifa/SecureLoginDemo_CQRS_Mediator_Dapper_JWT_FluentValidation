using FluentValidation;
using MadiatrProject.Attributes;
using MadiatrProject.Command;
using MadiatrProject.Enums;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MadiatrProject.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(permission:"StudentGet")]

public class StudentsController : ControllerBase
{
    private readonly IMediator _madiator;
    private readonly IValidator<StudetnIntCommand> _validator;
    private readonly IValidator<StudentUpdateCommand> _uvalidator;


    public StudentsController(IMediator mediator, IValidator<StudetnIntCommand> validator, IValidator<StudentUpdateCommand> uvalidator)
    {
            _madiator = mediator;
            _validator = validator;
            _uvalidator = uvalidator;
    }
    [HttpGet]
    [Authorize(PermissionEnum.StudentGet)]
    public  async Task<IActionResult> GetAllStudents()
    {
        
        var query = new GetAllStudentsQuery();
        var result =  await _madiator.Send(query); 
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> StudetnInsert(StudetnIntCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
            }

            return BadRequest(validationResult.Errors);
        }

        var result = await _madiator.Send(command);
        return Ok(result);

    }

    [HttpPut]
    public async Task<IActionResult> StudetnUpdate(StudentUpdateCommand command)
    {
        var validationResult = await _uvalidator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
            }

            return BadRequest(validationResult.Errors);
        }

        var result = await _madiator.Send(command);
        return Ok(result);

    }


}
