using Core.Dtos;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private StudentService studentService { get; set; }

        public AccountController(StudentService studentService)
        {
            this.studentService = studentService;
        }


        [HttpPost("register/student")]
        public IActionResult RegisterStudent(RegisterDto registerData)
        {
            studentService.Register(registerData);
            return Ok();
        }

       

    }
}
