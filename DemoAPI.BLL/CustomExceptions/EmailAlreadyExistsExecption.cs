using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.CustomExceptions
{
    public class EmailAlreadyExistsExecption : Exception
    {
        public EmailAlreadyExistsExecption() : base("Email already exists")
        {
        }

        public EmailAlreadyExistsExecption(string message) : base(message)
        {
        }
    }
}
