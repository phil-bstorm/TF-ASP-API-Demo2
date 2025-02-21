using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.Domain.Models;
using DemoAPI.DTOs;
using DemoAPI.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurService _utilisateurService;

        public UtilisateurController(IUtilisateurService utilisateurService)
        {
            _utilisateurService = utilisateurService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ListUtilisateurDTO>> GetAll() {
            IEnumerable<Utilisateur> utilisateurs = _utilisateurService.GetAll();

            IEnumerable<ListUtilisateurDTO> usersDTO = utilisateurs.Select(u => u.ToListUtilisateurDTO());

            return Ok(usersDTO);
        }

        [HttpPost]
        public ActionResult<DetailsUtilisateurDTO> Create([FromForm] CreateUtilisateurDTO dto) {
            if (ModelState.IsValid)
            {
                // return _utilisateurService.Create(dto.ToUtilisateur()).ToDetailsUtilisateurDTO();
                Utilisateur utilisateur = dto.ToUtilisateur();
                Utilisateur updated = _utilisateurService.Create(utilisateur);

                return Ok(updated.ToDetailsUtilisateurDTO());
            }

            return BadRequest();
        }

        [HttpGet("{id}")]
        public ActionResult<DetailsUtilisateurDTO> GetById(int id)
        {
            Utilisateur utilisateur = _utilisateurService.GetOne(id);
            DetailsUtilisateurDTO dto = utilisateur.ToDetailsUtilisateurDTO();
            return Ok(dto);
        }
    }
}
