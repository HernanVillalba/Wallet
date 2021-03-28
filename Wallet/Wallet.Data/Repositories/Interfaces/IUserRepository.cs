using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<Users>
    {
        bool FindEmail(string email);
        Task<Users> FindUser(string email);
        Task AddAccounts(Users user);
        IEnumerable<UserContact> GetByPage(int page);
        List<Users> Filter(UserFilterModel user);
    }
}
