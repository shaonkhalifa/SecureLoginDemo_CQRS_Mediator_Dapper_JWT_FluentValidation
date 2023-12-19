using FluentValidation;
using MadiatrProject.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadiatrProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IMediator _madiator;
    private readonly IValidator<CourseInsertCommand> _validator;



    public CoursesController(IMediator mediator, IValidator<CourseInsertCommand> validator)
    {
        _madiator = mediator;
        _validator = validator;
    }

    [HttpPost("CourseInsert")]
    public async Task<IActionResult> CourseInsert(CourseInsertCommand command)
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

}
