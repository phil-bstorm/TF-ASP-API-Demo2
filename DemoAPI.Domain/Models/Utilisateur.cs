using DemoAPI.Domain.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.Domain.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

        public required Grade Grade { get; set; }
    }
}
