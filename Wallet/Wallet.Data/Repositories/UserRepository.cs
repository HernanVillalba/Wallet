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

        public IEnumerable<UserContact> GetByPage(int page)
        {
            int pageSize = 10;
            return _context.Users.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new UserContact
                {
                       Id = p.Id,
                       FirstName = p.FirstName,
                       LastName = p.LastName,
                       Email = p.Email
                }
                ).ToList();
        }

        public List<Users> Filter(UserFilterModel user)
        {
            List<Users> list = new List<Users>();
            if (user.FirstName != "" && user.LastName != "" && user.Email != "") //cuando busca por nombre, apellido y email paramentros
            {
                list = _context.Users.Where(e => e.FirstName.ToLower() == user.FirstName.ToLower() && e.LastName.ToLower() == user.LastName.ToLower() && e.Email.ToLower() == user.Email.ToLower())
                    .ToList();
            }
            else if (user.FirstName != "" && user.LastName != "") //cuando busca por nombre y apellido
            {

                list = _context.Users.Where(e => e.FirstName.ToLower() == user.FirstName.ToLower() && e.LastName.ToLower() == user.LastName.ToLower())
                    .ToList();
            }
            else //cuando busca por cualquier parametro
            {
                list = _context.Users.Where(e => e.FirstName.ToLower() == user.FirstName.ToLower() || 
                                            e.LastName.ToLower() == user.LastName.ToLower() || 
                                            e.Email.ToLower() == user.Email.ToLower())
                    .ToList();
            }
            return list;
        }
    }
}
