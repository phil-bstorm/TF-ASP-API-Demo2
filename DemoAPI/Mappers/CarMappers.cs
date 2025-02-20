using DemoAPI.Domain.Models;
using DemoAPI.DTOs;

namespace DemoAPI.Mappers
{
    public static class CarMappers
    {
        public static Car ToCar(this CreateCarDTO dto)
        {
            return new Car
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Color = dto.Color,
                HorsePower = dto.HorsePower,
                IsNew = dto.IsNew,
            };
        }

        public static ListCarDTO ToListCarDTO(this Car car)
        {
            return new ListCarDTO
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
            };
        }
    }
}
