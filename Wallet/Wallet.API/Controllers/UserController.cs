using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Business.Logic;
using Wallet.Entities;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
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
                if (await _userBusiness.RegisterNewUser(newUser))
                {
                    return Ok(new { message = "Usuario registrado correctamente" });
                }
                else
                {
                    return BadRequest(new { message = "El usuario ya está registrado" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mostrar lista paginada de usuarios ordenada por apellido ascendente
        /// </summary>
        [Authorize]
        [HttpGet("{page:int:min(1)?}/{pageSize:int:min(1)?}")]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            try
            {
                return Ok(_userBusiness.PagedUsers(page, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
