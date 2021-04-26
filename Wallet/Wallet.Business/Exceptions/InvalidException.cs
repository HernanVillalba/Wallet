using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Business.Exceptions
{
    public class InvalidException : Exception
    {
        public InvalidException(string message) : base(message) { }
    }
}
