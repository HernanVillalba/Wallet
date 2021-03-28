using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wallet.Business.Logic;
using Wallet.Entities;

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
        
        /// <summary>
        /// Ingresar con email y contraseña para obtener token y poder utilizar la API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel userToCheck)
        {
            try
            {
                var token =  await _accessBusiness.LoginUser(userToCheck);
                return Ok(token);
            }
            catch
            {
                throw;
            }
        }
    }
}
