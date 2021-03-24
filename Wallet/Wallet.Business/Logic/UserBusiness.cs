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

        public IEnumerable<UserContact> PagedUsers(int page, int pageSize)
        {
            //            Users userToCheck= _mapper.Map<Users>(userToMap);            
            return _unitOfWork.Users.GetByPage(page, pageSize);
            
        }
    }
}
