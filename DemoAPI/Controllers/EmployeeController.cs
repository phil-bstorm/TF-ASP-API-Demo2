using DemoAPI.Database;
using DemoAPI.Database.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IFakeDatabase _fakeDatabase;

         public EmployeeController(IFakeDatabase fakeDatabase)
        {
            _fakeDatabase = fakeDatabase;
        }

        [HttpGet(Name ="Get all users")]
        public List<string> GetAllEmployees()
        {
            List<string> employees = _fakeDatabase.GetAllEmployees();

            // du traitement

            return employees;
        }

        [HttpPost(Name = "Ajout d'un employee")]
        public ActionResult<string> CreateEmployee([FromBody] string name)
        {
            _fakeDatabase.CreateEmployee(name);

            //return NoContent();
            return Ok(name);
        }
    }
}
