﻿using System;
using System.Threading.Tasks;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }
        IFixedTermDepositRepository FixedTermDeposits { get; }
        IAccountRepository Accounts { get; }
        ITransactionLogRepository TransactionLog { get; }
        IRatesRepository Rates { get; }
        IRefundRequestRepository RefundRequest { get; }
        ITransfersRepository Transfers { get; }
        IEmailTemplatesRepository EmailTemplates { get; }
        Task<int> Complete();
    }
}
