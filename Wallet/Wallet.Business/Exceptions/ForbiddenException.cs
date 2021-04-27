using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Business.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
