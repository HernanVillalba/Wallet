using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class EmailTemplatesRepository : GenericRepository<EmailTemplates>, IEmailTemplatesRepository
    {
        public EmailTemplatesRepository(WALLETContext context) : base(context)
        {

        }
    }
}
