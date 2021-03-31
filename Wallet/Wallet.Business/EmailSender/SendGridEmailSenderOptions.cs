using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Business.EmailSender
{
    public class SendGridEmailSenderOptions
    {
        public string ApiKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}
