using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

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
    }
}
