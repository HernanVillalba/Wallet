using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class TrasactionRepository : GenericRepository<Transactions>, ITransactionRepository
    {
        public TrasactionRepository(WALLETContext context) : base(context)
        {

        }
    }
}
