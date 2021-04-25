using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Data.Repositories
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        public UserRepository(WALLETContext context) : base(context)
        {
        }
        public bool FindEmail(string email)
        {
            return _context.Users.Any(user => user.Email == email);
        }

        public async Task<Users> FindUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task AddAccounts(Users user)
        {
            Accounts ars = new Accounts
            {
                Currency = "ARS",
                UserId = user.Id
            };
            Accounts usd = new Accounts
            {
                Currency = "USD",
                UserId = user.Id
            };
            user.Accounts.Add(usd);
            user.Accounts.Add(ars);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<UserContact> GetByPage(int page, UserFilterModel user)
        {
            int pageSize = 10;
            return _context.Users                
                .Where(e => (string.IsNullOrEmpty(user.FirstName) || e.FirstName.ToLower() == user.FirstName.ToLower()) &&
                            (string.IsNullOrEmpty(user.LastName) || e.LastName.ToLower() == user.LastName.ToLower()) &&
                            (string.IsNullOrEmpty(user.Email) || e.Email.ToLower() == user.Email.ToLower()))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(u => u.LastName)
                .Select(p => new UserContact
                {
                       Id = p.Id,
                       FirstName = p.FirstName,
                       LastName = p.LastName,
                       Email = p.Email
                }
                ).ToList();
        }
    }
}
