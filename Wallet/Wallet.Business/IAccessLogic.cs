using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.ModelsAPI;

namespace Wallet.Business
{
    public interface IAccessLogic
    {
        Task<bool> RegisterNewUser(Users user);
        Task<object> LoginUser(Users userToCheck);
    }
}
