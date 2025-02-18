﻿using DemoAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DAL.Repositories.Interfaces
{
    public interface ICarRepository
    {
        IEnumerable<Car> GetAll();
        Car Create(Car car);
    }
}
