using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class RatesRepository : GenericRepository<Rates>, IRatesRepository
    {
        public RatesRepository(WALLETContext context) : base(context)
        {
        }

        public Rates GetLastValues()
        {
            return _context.Rates.OrderByDescending(r => r.Date).FirstOrDefault();
        }
    }
}
