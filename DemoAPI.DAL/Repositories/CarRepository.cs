using DemoAPI.DAL.Database;
using DemoAPI.DAL.Repositories.Interfaces;
using DemoAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
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

        public Car? GetOne(int id)
        {
            //return _context.Cars.Where(c => c.Id == id).FirstOrDefault();
            //return _context.Cars.FirstOrDefault(x => x.Id == id);
            return _context.Cars.Include(c => c.Owner)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }
    }
}
