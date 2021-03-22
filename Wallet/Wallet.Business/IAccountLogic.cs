using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Business
{
    public interface IAccountLogic
    {
        List<double> SelectBalances(int id, params string[] currency);
    }
}
