using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Odata6Demo.Api.Models;
using Odata6Demo.Api.Services;

namespace Odata6Demo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // this controller is dependent on StudentService, which implements and inherits from the IStudentService interface
        private readonly IStudentService studentService;

        // use dependency injection from Startup.cs to inject implementation of IStudentService into this controller upon construction
        public StudentsController(IStudentService studentService)
        {
            this.studentService = studentService;
        }

        [HttpGet]
        public ActionResult<IQueryable<Student>> GetAllStudents()
        {
            IQueryable<Student> retrievedStudents = this.studentService.RetrieveAllStudents();
            return Ok(retrievedStudents);
        }
    }
}
