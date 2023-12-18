using FluentValidation;
using MadiatrProject.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadiatrProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly IMediator _madiator;
        private readonly IValidator<PasswordResetCommand> _validator;
        public PasswordResetController(IMediator madiator, IValidator<PasswordResetCommand> validator)
        {
            _madiator = madiator;
            _validator = validator;
        }
        [HttpPatch("PasswordReset")]
        public async Task<IActionResult> PasswordReset(PasswordResetCommand command)
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
