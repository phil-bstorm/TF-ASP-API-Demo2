using DemoAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.Services.Interfaces
{
    public interface IService<Entity, Key>
    {
        IEnumerable<Entity> GetAll();
        Entity Create(Entity entity);
        Entity? GetOne(Key id);
    }
}
