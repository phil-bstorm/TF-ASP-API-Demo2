using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Car>> GetAll()
        {
            IEnumerable<Car> cars = _carService.GetAll();

            // transformer DTO

            return Ok(cars);
        }

        [HttpPost]
        public ActionResult<Car> Create([FromBody] Car car)
        {
            Car added = _carService.Create(car);

            // transformer DTO

            return Ok(added);
        }
    }
}
