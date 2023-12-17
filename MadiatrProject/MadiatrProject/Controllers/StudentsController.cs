using FluentValidation;
using MadiatrProject.Attributes;
using MadiatrProject.Command;
using MadiatrProject.Model;
using MadiatrProject.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MadiatrProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _madiator;
        private readonly IValidator<StudetnIntCommand> _validator;

        public StudentsController(IMediator mediator, IValidator<StudetnIntCommand> validator)
        {
                _madiator = mediator;
                _validator = validator;
        }
        [HttpGet]
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


    }
}
