using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Models;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Business.Operations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccessController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]Users user)
        {
            if (!_unitOfWork.Users.FindEmail(user.Email))
            {
                try
                {
                    user.Password = PasswordHash.Generate(user.Password);
                    _unitOfWork.Users.Insert(user);
                    await _unitOfWork.Users.AddAccounts(user);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch(Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return BadRequest(new { message = "El usuario ya está registrado" });
        }
    }
}
