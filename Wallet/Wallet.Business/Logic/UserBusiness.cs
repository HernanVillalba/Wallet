using AutoMapper;
using System;
using System.Collections.Generic;
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

        public async Task<bool> RegisterNewUser(RegisterModel newUser)
        {
            Users user = _mapper.Map<Users>(newUser);
            if (!_unitOfWork.Users.FindEmail(user.Email))
            {
                user.Password = PasswordHash.Generate(user.Password);
                _unitOfWork.Users.Insert(user);
                await _unitOfWork.Users.AddAccounts(user);
                await _unitOfWork.Complete();
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserContact GetUserDetails(int userId)
        {
            if (userId <= 0)
                throw new CustomException(400, "Id inválido");

            // Search the user by id in the database
            Users userDB = _unitOfWork.Users.GetById(userId);
            
            // Map the user database model to user view model (without password for example)
            UserContact user = _mapper.Map<UserContact>(userDB);

            return user;
        }

        public IEnumerable<UserContact> PagedUsers(int page)
        {
            return _unitOfWork.Users.GetByPage(page);            
        }

        public List<UserFilterModel> Filter(UserFilterModel user)
        {
            var listDB = _unitOfWork.Users.Filter(user);
            List<UserFilterModel> listFilter = _mapper.Map<List<UserFilterModel>>(listDB);
            return listFilter;
        }
    }
}
