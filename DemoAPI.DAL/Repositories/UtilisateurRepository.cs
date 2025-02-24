using DemoAPI.DAL.Database;
using DemoAPI.DAL.Repositories.Interfaces;
using DemoAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.DAL.Repositories
{
    public class UtilisateurRepository : IUtilisateurRepository
    {
        private readonly DemoDbContext _context;

        public UtilisateurRepository(DemoDbContext context)
        {
            _context = context;
        }

        public Utilisateur Create(Utilisateur utilisateur)
        {
            Utilisateur added = _context.Utilisateurs.Add(utilisateur).Entity;
            _context.SaveChanges();
            return added;
        }

        public void Delete(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Remove(utilisateur);
            _context.SaveChanges();
        }

        public IEnumerable<Utilisateur> GetAll(int offset, int limit)
        {
            return _context.Utilisateurs
                .Skip(offset)
                .Take(limit);
        }

        public Utilisateur? GetByEmail(string email)
        {
            return _context.Utilisateurs.FirstOrDefault(x => x.Email == email);
        }

        public Utilisateur? GetOne(int id)
        {
            return _context.Utilisateurs
                .Include(u => u.Cars) // permet de récupérer les voitures de l'utilisateur
                .AsNoTracking() // si le lazyloading est désactivé dans la configuration, permet de ne récupérer que le premier sous-objet
                .FirstOrDefault(x => x.Id == id);
        }

        public Utilisateur Update(Utilisateur utilisateur)
        {
            Utilisateur updated = _context.Utilisateurs.Update(utilisateur).Entity;
            _context.SaveChanges();
            return updated;
        }
    }
}
