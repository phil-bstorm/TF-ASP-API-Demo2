using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.Domain.Models
{
    public class CarTag
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public List<Car> Cars { get; set; } = [];
    }
}
