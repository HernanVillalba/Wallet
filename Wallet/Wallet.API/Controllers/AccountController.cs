using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Wallet.Business.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Repositories.Interfaces;
using Wallet.API.Models;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
                BalanceModel balance = new BalanceModel()
                {
                    argBalance = _unitOfWork.Accounts.GetAccountBalance(id, "ARS"),
                    usdBalance = _unitOfWork.Accounts.GetAccountBalance(id, "USD")
                };
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
