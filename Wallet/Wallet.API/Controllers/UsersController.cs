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
    public class UsersController : Controller
    {
        private readonly IUserBusiness _userBusiness;
        public UsersController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        /// <summary>
        /// Registrar un usuario nuevo con un email único
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel newUser)
        {
            try
            {
                await _userBusiness.RegisterNewUser(newUser);              
                return StatusCode(201);               
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
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Mostrar lista paginada de usuarios ordenada por apellido con filtros opcionales
        /// </summary>
        /// <param name="page">Página a mostrar</param>
        /// <param name="user"></param>
        [Authorize]
        [HttpGet]
        public IActionResult GetUsersByPage([FromQuery]int page, [FromQuery]UserFilterModel user)
        {
            try
            { 
                return Ok(_userBusiness.PagedUsers(page, user));
            }
            catch 
            {
                throw;
            }        
        }
    }
}
