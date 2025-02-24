using DemoAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DAL.Repositories.Interfaces
{
    public interface IUtilisateurRepository
    {
        IEnumerable<Utilisateur> GetAll(int offset, int limit);
        Utilisateur Create(Utilisateur utilisateur);
        Utilisateur? GetOne(int id);
        Utilisateur? GetByEmail(string email);
        Utilisateur Update(Utilisateur utilisateur);
        void Delete(Utilisateur utilisateur);

    }
}
