using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Wallet.Entities;
using AutoMapper;
using Wallet.Business.Logic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccessBusiness _accessBusiness;

        public AccessController(IMapper mapper, IAccessBusiness accessBusiness)
        {
            _mapper = mapper;
            _accessBusiness= accessBusiness;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel newUser)
        {
            try
            {               
                if (await _accessBusiness.RegisterNewUser(newUser))
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel userToCheck)
        {
            try
            {
                var token =  await _accessBusiness.LoginUser(userToCheck);
                if(token != null)
                {
                    return Ok(token);
                }
                else
                {
                    return BadRequest(new { message = "Los datos ingresados son incorrectos" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
