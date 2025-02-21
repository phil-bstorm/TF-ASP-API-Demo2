using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.DAL.Repositories.Interfaces;
using DemoAPI.Domain.Models;
using Isopoh.Cryptography.Argon2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPI.BLL.Services
{
    public class UtilisateurService : IUtilisateurService
    {
        private readonly IUtilisateurRepository _utilisateurRepository;

        public UtilisateurService(IUtilisateurRepository utilisateurRepository)
        {
            _utilisateurRepository = utilisateurRepository;
        }

        public Utilisateur Create(Utilisateur entity)
        {
            // vérifier que mon email n'est pas déjà en DB
            Utilisateur? existingEmail = _utilisateurRepository.GetByEmail(entity.Email);
            if (existingEmail is not null)
            {
                throw new Exception("Email already exists");
            }

            // encryption du mot de passe
            entity.Password = Argon2.Hash(entity.Password);

            return _utilisateurRepository.Create(entity);
        }

        public void Delete(int id)
        {
            Utilisateur? utilisateur = _utilisateurRepository.GetOne(id);
            if (utilisateur is not null)
            {
                _utilisateurRepository.Delete(utilisateur);
            }
        }

        public IEnumerable<Utilisateur> GetAll()
        {
            return _utilisateurRepository.GetAll();
        }

        public Utilisateur? GetOne(int id)
        {
            return _utilisateurRepository.GetOne(id);
        }

        public Utilisateur Update(Utilisateur val)
        {
            Utilisateur? utilisateur = _utilisateurRepository.GetOne(val.Id);
            if(utilisateur is not null)
            {
                utilisateur.Username = val.Username is null ? utilisateur.Username : val.Username;
                utilisateur.Password = val.Password is null ? utilisateur.Password : Argon2.Hash(val.Password); // on n'oublie pas de re-crypter le mot de passe

                Utilisateur updated = _utilisateurRepository.Update(utilisateur);
                return updated;
            }
            else
            {
                throw new Exception("Utilisateur not found");
            }
        }

        public Utilisateur Login(string email, string password)
        {
            // récupérer l'utilisateur qui match le user
            Utilisateur? utilisateur = _utilisateurRepository.GetByEmail(email);

            if(utilisateur is not null)
            {
                // "Verfy" de Argon2
                if(Argon2.Verify(utilisateur.Password, password))
                {
                    return utilisateur;
                }
            }

            throw new Exception("Invalid login");
        }
    }
}
