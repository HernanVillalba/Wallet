using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Business.Logic;
using Wallet.Entities;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserBusiness _userBusiness;
        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        /// <summary>
        /// Registrar un usuario nuevo con un email único
        /// </summary>
        /// <response code="200">Usuario registrado correctamente</response>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel newUser)
        {
            try
            {
                await _userBusiness.RegisterNewUser(newUser);              
                return Ok(new { message = "Usuario registrado correctamente" });               
            }
            catch 
            {
                throw;
            }
        }

        /// <summary>
        /// Obtener detalles de un usuario en específico por su id
        /// </summary>
        /// <param name="userId">Id del usuario</param>
        [Authorize]
        [HttpGet("{userId}")]
        public IActionResult GetUserById(int userId)
        {
            try
            {
                return Ok(_userBusiness.GetUserDetails(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mostrar lista paginada de usuarios ordenada por apellido ascendente
        /// </summary>
        /// <param name="page">Página a mostrar</param>
        [Authorize]
        [HttpGet("Page/{page}")]
        public IActionResult GetAll(int page)
        {
            try
            { 
                return Ok(_userBusiness.PagedUsers(page));
            }
            catch 
            {
                throw;
            }
        }

        [Authorize]
        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] UserFilterModel user)
        {
            try
            {
                var list = _userBusiness.Filter(user);
                if(list.Count() > 0) { return Ok(list); }
                else { return BadRequest("No se encontraron usuarios"); }
            }
            catch (Exception ex){ return BadRequest(ex.Message); }
        }
    }
}
