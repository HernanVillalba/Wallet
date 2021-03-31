using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class TransactionLogRepository : GenericRepository<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(WALLETContext context) : base(context)
        {

        }
    }
}
