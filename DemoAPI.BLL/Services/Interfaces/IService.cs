using DemoAPI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.Services.Interfaces
{
    public interface IService<T>
    {
        IEnumerable<T> GetAll();
        T Create(T entity);
    }
}
