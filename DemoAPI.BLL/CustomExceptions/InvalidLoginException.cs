using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.CustomExceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException() : base("Invalid email or password") { }
    }
}
