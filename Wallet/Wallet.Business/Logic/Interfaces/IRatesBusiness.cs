using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface IRatesBusiness
    {
        Task<Rates> GetRates();
        Task<Rates> SetRates();
        IEnumerable<RateModel> GetLatestRates();
    }
}
