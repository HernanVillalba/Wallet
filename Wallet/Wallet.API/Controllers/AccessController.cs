using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Wallet.API.Models;
using Wallet.Business;
using Wallet.Data.ModelsAPI;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccessLogic _accessLogic;

        public AccessController(IMapper mapper, IAccessLogic accessLogic)
        {
            _mapper = mapper;
            _accessLogic = accessLogic;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel newUser)
        {
            Users user = _mapper.Map<Users>(newUser);
            try
            {               
                if (await _accessLogic.RegisterNewUser(user))
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
            Users mappedUser = _mapper.Map<Users>(userToCheck);
            try
            {
                var token =  await _accessLogic.LoginUser(mappedUser);
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
