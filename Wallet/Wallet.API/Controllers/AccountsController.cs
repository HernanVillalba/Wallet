using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Wallet.Business;
using Wallet.Business.Logic;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountBusiness _accountBusiness;

        public AccountsController(IAccountBusiness accountBusiness)
        {
            _accountBusiness= accountBusiness;
        }

        /// <summary>
        /// Mostrar lista de cuentas del usuario actual con sus respectivos balances
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult ListAccounts()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
                var accounts = _accountBusiness.GetAccountsWithBalance(id);
                return Ok(accounts);
            }
            catch
            {
                throw new CustomException(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Mostrar balances de las cuentas en pesos y dolares del usuario actual
        /// </summary>
        [Authorize]
        [HttpGet("{balance}")]
        public IActionResult ListBalance()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
                var balances = _accountBusiness.GetBalances(id);               
                return Ok(balances);
            }
            catch
            {
                throw new CustomException(500, "Error interno del servidor");
            }
        }        
    }
}

    
