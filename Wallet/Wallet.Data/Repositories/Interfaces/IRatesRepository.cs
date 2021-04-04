using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IRatesRepository : IGenericRepository<Rates>
    {
        Rates GetLastValues();
    }
}
