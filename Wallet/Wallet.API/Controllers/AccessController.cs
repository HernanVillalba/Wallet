using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Models;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Business.Operations;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccessController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterModel newUser)
        {
            Users user = _mapper.Map<Users>(newUser);
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
