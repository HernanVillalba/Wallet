using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class AccessBusiness : IAccessBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AccessBusiness(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }       

        public async Task<object> LoginUser(LoginModel userToMap)
        {
            Users userToCheck= _mapper.Map<Users>(userToMap);
            //Verify that the user exists and the password is correct
            var user = await _unitOfWork.Users.FindUser(userToCheck.Email);
            if (user != null && PasswordHash.VerifyPassword(user.Password, userToCheck.Password))
            {
                //use secret key to create token for authentication
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
                return new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    tokenString
                };
            }
            //if user or password is incorrect
            else
            {
                return null;
            }
        }
    }
}
