using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.Domain.Models;
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
        private readonly IUtilisateurService _utilisateurService;

        public CarController(ICarService carService, IUtilisateurService utilisateurService)
        {
            _carService = carService;
            _utilisateurService = utilisateurService;
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
            // TODO attention le getOne peut retrouner NULL (Gerer erreur 404) => cours sur les middlewares
            Car car = _carService.GetOne(id);

            // TODO transformation en DTO

            return Ok(car);
        }

        [HttpPost]
        public ActionResult<Car> Create([FromBody] CreateCarDTO DTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Car car = DTO.ToCar();

            if(DTO.OwnerId is not null)
            {
                car.Owner = _utilisateurService.GetOne(DTO.OwnerId.Value);
            }

            Car added = _carService.Create(car);

            // TODO transformer DTO

            return Ok(added);
        }
    }
}
