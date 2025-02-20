using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.DAL.Repositories.Interfaces;
using DemoAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public Car Create(Car entity)
        {
            if(entity.HorsePower > 999)
            {
                throw new Exception("Too many horse power");
            }
           Car added = _carRepository.Create(entity);
            return added;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Car> GetAll()
        {
            IEnumerable<Car> cars = _carRepository.GetAll();
            return cars;
        }

        public Car? GetOne(int id)
        {
            return _carRepository.GetOne(id);
        }

        public Utilisateur Update(Utilisateur val)
        {
            throw new NotImplementedException();
        }
    }
}
