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
        private readonly IAccessBusiness _accessBusiness;

        public AccessController( IAccessBusiness accessBusiness)
        {
            _accessBusiness= accessBusiness;
        }              

        [HttpPost]
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
