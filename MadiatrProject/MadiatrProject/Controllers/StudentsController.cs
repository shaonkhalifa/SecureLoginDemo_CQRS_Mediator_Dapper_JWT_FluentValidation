using MadiatrProject.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadiatrProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _madiator;

        public StudentsController(IMediator mediator)
        {
                _madiator = mediator;
        }
        [HttpGet]
        public  async Task<IActionResult> GetAllStudents()
        {
            var query = new GetAllStudentsQuery();
            var result =  await _madiator.Send(query); 
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> StudetnInsert()
        //{

        //}


    }
}
