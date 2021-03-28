using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Business
{
    public class CustomException : Exception
    {
        public CustomException() 
        {
            Error = Message;
        }
        public CustomException(int statusCode, string error)
        {
            StatusCode = statusCode;
            Error = error;
        }
        public int StatusCode { get; set; }
        public string Error { get; set; }
    }
}
