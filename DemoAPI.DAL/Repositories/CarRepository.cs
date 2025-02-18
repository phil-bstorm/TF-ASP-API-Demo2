using DemoAPI.DAL.Database;
using DemoAPI.DAL.Repositories.Interfaces;
using DemoAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DAL.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly DemoDbContext _context;

        public CarRepository(DemoDbContext context)
        {
            _context = context;
        }

        public Car Create(Car car)
        {
            Car result = _context.Cars.Add(car).Entity;
            _context.SaveChanges();
            return result;
        }

        public IEnumerable<Car> GetAll()
        {
            return _context.Cars;
        }
    }
}
