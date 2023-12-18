using FluentValidation;
using MadiatrProject.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadiatrProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _madiator;
        private readonly IValidator<UserRegisterCommand> _validator;
        public AuthenticationController(IMediator madiator, IValidator<UserRegisterCommand> validator)
        {
            _madiator = madiator;
            _validator = validator;
        }

        [HttpPost("UserRegister")]
        public async Task<IActionResult> UserRegister(UserRegisterCommand command)
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
        [HttpPost("UserSignIN")]
        public async Task<IActionResult> UserSignIn(UserSignInCommand command)
        {
            //var validationResult = await _validator.ValidateAsync(command);

            //if (!validationResult.IsValid)
            //{
            //    foreach (var error in validationResult.Errors)
            //    {
            //        Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
            //    }

            //    return BadRequest(validationResult.Errors);
            //}

            var result = await _madiator.Send(command);
            return Ok(result);

        }

        [HttpPost("PermissionAssign")]
        public async Task<IActionResult> PermissionAssign(RolePermissionSetupCommand command)
        {
         
            var result = await _madiator.Send(command);
            return Ok(result);

        }
    }
}
