using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.Domain;
using DemoAPI.DTOs;
using DemoAPI.Mappers;
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
        public ActionResult<IEnumerable<ListCarDTO>> GetAll()
        {
            IEnumerable<Car> cars = _carService.GetAll();

            // transformer DTO
            IEnumerable<ListCarDTO> carsDTO = cars.Select(c => c.ToListCarDTO());

            return Ok(carsDTO);
        }

        [HttpGet("{id}")]
        public ActionResult<Car> Get(int id)
        {
            // TODO attention le getOne peut retrouner NULL (Gerer erreur 404)
            Car car = _carService.GetOne(id);

            // transformation en DTO

            return Ok(car);
        }

        [HttpPost]
        public ActionResult<Car> Create([FromBody] CreateCarDTO DTO)
        {
            // model state is valid

            Car car = DTO.ToCar();
            Car added = _carService.Create(car);

            // transformer DTO

            return Ok(added);
        }
    }
}
