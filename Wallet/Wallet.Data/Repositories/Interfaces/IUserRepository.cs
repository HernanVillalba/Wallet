﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        bool FindEmail(string email);
        Task<Users> FindUser(string email);
        Task AddAccounts(Users user);
        IEnumerable<UserContact> GetByPage(int page);
    }
}
