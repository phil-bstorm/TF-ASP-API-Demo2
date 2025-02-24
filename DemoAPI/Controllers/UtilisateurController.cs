using DemoAPI.BLL.Services.Interfaces;
using DemoAPI.Domain.CustomEnums;
using DemoAPI.Domain.Models;
using DemoAPI.DTOs;
using DemoAPI.Mappers;
using DemoAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UtilisateurController : ControllerBase
    {
        private readonly IUtilisateurService _utilisateurService;
        private readonly IAuthService _authService;
        private readonly IMailHelperService _mailHelperService;

        public UtilisateurController(IUtilisateurService utilisateurService, IAuthService authService, IMailHelperService mailHelperService)
        {
            _utilisateurService = utilisateurService;
            _authService = authService;
            _mailHelperService = mailHelperService;
        }

        [HttpGet]
        [Authorize(Roles= "Medior")]
        public ActionResult<IEnumerable<ListUtilisateurDTO>> GetAll([FromQuery] PaginationParams pagination) {
            IEnumerable<Utilisateur> utilisateurs = _utilisateurService.GetAll(pagination);

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

                _mailHelperService.SendWelcomeMail(utilisateur);

                return Ok(updated.ToDetailsUtilisateurDTO());
            }

            return BadRequest();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Junior,Medior")]
        public ActionResult<DetailsUtilisateurDTO> GetById(int id)
        {
            Utilisateur utilisateur = _utilisateurService.GetOne(id);
            DetailsUtilisateurDTO dto = utilisateur.ToDetailsUtilisateurDTO();
            return Ok(dto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Medior")]
        public ActionResult<DetailsUtilisateurDTO> Update(int id, [FromForm] UpdateUtilisateurDTO dto)
        {
            if (ModelState.IsValid)
            {
                Utilisateur utilisateur = dto.ToUtilisateur();
                utilisateur.Id = id;

                Utilisateur updated = _utilisateurService.Update(utilisateur);
                return Ok(updated.ToDetailsUtilisateurDTO());
            }
            return BadRequest();
        }

        [HttpPut()]
        public ActionResult<DetailsUtilisateurDTO> UpdateSelf([FromForm] UpdateUtilisateurDTO dto)
        {
           if(int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value, out int id)) 
            {
                if (ModelState.IsValid)
                {
                    // convertion du DTO
                    Utilisateur utilisateur = dto.ToUtilisateur();
                    utilisateur.Id = id;

                    Utilisateur updated = _utilisateurService.Update(utilisateur);
                    return Ok(updated.ToDetailsUtilisateurDTO());
                }
            }
            return BadRequest();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<string> Login([FromForm] LoginUtilisateurDTO dto)
        {
            // login BLL
            Utilisateur utilisateur = _utilisateurService.Login(dto.Email, dto.Password);

            // génération du token
            string token = _authService.GenerateToken(utilisateur);

            _mailHelperService.SendWarningLoginMail(utilisateur);

            // envoie de la réponse
            return Ok(token);
        }
    }
}