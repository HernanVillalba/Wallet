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
            // Search the user by id in the database
            Users userDB = _unitOfWork.Users.GetById(userId);
            
            // Map the user database model to user view model (without password for example)
            UserContact user = _mapper.Map<UserContact>(userDB);

            return user;
        }

        public IEnumerable<UserContact> PagedUsers(int page)
        {
            if (page < 1)
            {
                throw new CustomException(400, "Ingrese un valor mayor a cero");
            }
            var users = _unitOfWork.Users.GetByPage(page);  
            if (users.Any())
            {
                return users;
            }
            else
            {
                throw new CustomException(404, "Página no encontrada");
            }

        }

        public List<UserFilterModel> Filter(UserFilterModel user)
        {
            var listDB = _unitOfWork.Users.Filter(user);
            List<UserFilterModel> listFilter = _mapper.Map<List<UserFilterModel>>(listDB);
            return listFilter;
        }
    }
}
