using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class FixedTermDepositRepository : GenericRepository<FixedTermDeposit>, IFixedTermDepositRepository
    {
        public FixedTermDepositRepository(WALLETContext context) : base(context) {}

        public IEnumerable<FixedTermDeposit> ExecuteStoredProcedure(string stored_procedure)
        {
            return _context.FixedTermDeposit.FromSqlRaw("EXEC " + stored_procedure);
        }
    }
}
