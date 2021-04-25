using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task RegisterNewUser(RegisterModel newUser)
        {
            Users user = _mapper.Map<Users>(newUser);
            if (!_unitOfWork.Users.FindEmail(user.Email))
            {
                user.Password = PasswordHash.Generate(user.Password);
                _unitOfWork.Users.Insert(user);
                await _unitOfWork.Users.AddAccounts(user);
                await _unitOfWork.Complete();
            }
            else
            {
                throw new CustomException(400, "Usuario ya registrado");
            }
        }

        public UserContact GetUserDetails(int userId)
        {
            if (userId <= 0)
                throw new CustomException(400, "Id inválido");

            // Search the user by id in the database
            Users userDB = _unitOfWork.Users.GetById(userId);
            if (userDB == null)
            {
                throw new CustomException(404, "Usuario no encontrado");
            }
            // Map the user database model to user view model (without password for example)
            UserContact user = _mapper.Map<UserContact>(userDB);

            return user;
        }

        public IEnumerable<UserContact> PagedUsers(int page, UserFilterModel user)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var users = _unitOfWork.Users.GetByPage(page, user);  
            if (users.Any())
            {
                return users;
            }
            else
            {
                throw new CustomException(404, "Página no encontrada");
            }
        }        
    }
}
