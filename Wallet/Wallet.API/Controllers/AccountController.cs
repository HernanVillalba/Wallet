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
using Wallet.Data.Models;

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
        [HttpGet("balance")]
        public IActionResult ListBalance()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
                BalanceModel balance = new BalanceModel()
                {
                    ArgBalance = _unitOfWork.Accounts.GetAccountBalance(id, "ARS"),
                    UsdBalance = _unitOfWork.Accounts.GetAccountBalance(id, "USD")
                };
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult ListAccounts()
        {
            var id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            var accounts = _unitOfWork.Accounts.GetUserAccounts(id);
            List<AccountModel> acc = new List<AccountModel>();
            try
            {
                foreach (Accounts a in accounts)
                {
                    acc.Add(new AccountModel
                    {
                        Id = a.Id,
                        Currency = a.Currency,
                        Balance = _unitOfWork.Accounts.GetAccountBalance(id, a.Currency)
                    });
                }
                return Ok(acc);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

    
