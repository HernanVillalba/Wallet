using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Wallet.Business.Logic;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountBusiness _accountBusiness;

        public AccountController(IAccountBusiness accountBusiness)
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
                return Ok(_accountBusiness.GetAccountsWithBalance(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mostrar balances de las cuentas en pesos y dolares del usuario actual
        /// </summary>
        [Authorize]
        [HttpGet("balance")]
        public IActionResult ListBalance()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
                var balances = _accountBusiness.GetBalances(id);               
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }        
    }
}

    
