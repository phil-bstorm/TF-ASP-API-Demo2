using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.CustomExceptions
{
    public class UtilisateurNotFoundException : Exception
    {
        public UtilisateurNotFoundException() : base("Utilisateur not found.") { }

        public UtilisateurNotFoundException(string message) : base(message) { }
    }
}
