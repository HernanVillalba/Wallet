using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wallet.Business.Logic;
using Wallet.Entities;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionBusiness _sessionBusiness;

        public SessionController( ISessionBusiness sessionBusiness)
        {
            _sessionBusiness= sessionBusiness;
        }   
        
        /// <summary>
        /// Ingresar con email y contraseña para obtener token y poder utilizar la API
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel userToCheck)
        {
            try
            {
                var token =  await _sessionBusiness.LoginUser(userToCheck);
                return Ok(token);
            }
            catch
            {
                throw;
            }
        }
    }
}
