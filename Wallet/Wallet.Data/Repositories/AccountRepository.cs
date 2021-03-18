using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Wallet.Data.Repositories
{
    public class AccountRepository : GenericRepository<Accounts>, IAccountRepository 
    {
        public AccountRepository(WALLETContext context) : base(context)
        {
        }        
    }
}
