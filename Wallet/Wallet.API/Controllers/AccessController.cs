using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Models;
using Wallet.Data.ModelsAPI;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Business.Operations;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccessController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("register")]
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

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginModel userToCheck)
        {
            Users mappedUser = _mapper.Map<Users>(userToCheck);
            try
            {
                var user = _unitOfWork.Users.FindUser(mappedUser.Email);
                if (user != null && PasswordHash.VerifyPassword(user.Password, userToCheck.Password))
                {
                    var secretKey = _configuration.GetValue<string>("SecretKey");
                    var key = Encoding.ASCII.GetBytes(secretKey);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("UserId", user.Id.ToString()),
                        }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var createdToken = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(createdToken);
                    return Ok(new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        tokenString
                    });
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
