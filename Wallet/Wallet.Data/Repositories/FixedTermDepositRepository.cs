using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class FixedTermDepositRepository : GenericRepository<FixedTermDeposits>, IFixedTermDepositRepository
    {
        public FixedTermDepositRepository(WALLETContext context) : base(context) {}

        public IEnumerable<FixedTermDeposits> GetAllByUserId(int userId)
        {
            // Executes the stored procedure to retrieve all the fixed term deposits related to an user
            return _context.FixedTermDeposits.FromSqlRaw($"EXEC SP_GetUserFixedTermDeposits {userId}");
        }
    }
}
