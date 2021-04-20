using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;

namespace Wallet.Test
{
    public static class DataInitializer
    {
        public static readonly List<object> lista = new()
        {
            #region Users
            new Users
            {
                Id = 1,
                FirstName = "Juanpi",
                LastName = "Taladro",
                Email = "jt11@mail.com",
                Password = "Pass1234!"
            },
            new Users
            {
                Id = 2,
                FirstName = "Leon",
                LastName = "Kennedy",
                Email = "leonAlkemy@yopmail.com",
                Password = "123!"
            },
            #endregion
            #region Accounts
            new Accounts
            {
                Id = 1,
                Currency = "USD",
                UserId = 1
            },
            new Accounts
            {
                Id = 2,
                Currency = "ARS",
                UserId = 1
            },
            new Accounts
            {
                Id = 3,
                Currency = "USD",
                UserId = 2
            },
            new Accounts
            {
                Id = 4,
                Currency = "ARS",
                UserId = 2
            },
            #endregion
            #region Categories
            new Categories
            {
                Id = 1,
                Editable = true,
                Type = "Regular"
            },
            new Categories
            {
                Id = 2,
                Editable = false,
                Type = "Buy currency"
            },
            new Categories
            {
                Id = 3,
                Editable = false,
                Type = "Fixed term deposit"
            },
            new Categories
            {
                Id = 4,
                Editable = false,
                Type = "Transfer"
            },
            #endregion
            #region Transactions
            new Transactions
            {
                Id = 1,
                Amount = 100,
                Concept = "Recarga por defecto",
                Type = "Topup",
                AccountId = 2,
                Date = DateTime.Now,
                CategoryId = 1
            },
            new Transactions
            {
                Id = 2,
                Amount = 155,
                Concept = "Recarga por defecto 2",
                Type = "Topup",
                AccountId = 4,
                Date = DateTime.Now,
                CategoryId = 1
            },
            new Transactions
            {
                Id = 10,
                Amount = 100,
                Concept = "Transferencia a la cuenta 3",
                Type = "Topup",
                AccountId = 1,
                Date = DateTime.Now,
                CategoryId = 4
            },
            new Transactions
            {
                Id = 11,
                Amount = 100,
                Concept = "Transferencia de cuenta 1",
                Type = "Topup",
                AccountId = 3,
                Date = DateTime.Now,
                CategoryId = 4
            },
            #endregion
            #region Transfers
            new Transfers
            {
                Id = 1,
                OriginTransactionId = 10,
                DestinationTransactionId = 11
            },
            #endregion
        };
        public static void Initialize(WALLETContext _context)
        {
            _context.AddRange(lista);
            _context.SaveChanges();
        }
    }
}
