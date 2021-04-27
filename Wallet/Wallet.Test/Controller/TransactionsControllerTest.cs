using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business.Logic;
using Wallet.Data.Models;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test.Business
{
    public class TransactionsControllerTest : TestBase
    {
        // Objects declaration
        static TransactionsController _controller;
        static TransactionCreateModel createModel = new()
        {
            Amount = 100,
            Concept = "Recarga default",
            Type = "Topup"
        };
        TransactionFilterModel filterModel = new();

        // Ctor
        public TransactionsControllerTest()
        {
            var _transactionsBusiness = new TransactionBusiness(_unitOfWork, _mapper, _ratesBusiness, _accountBusiness);
            _controller = new TransactionsController(_transactionsBusiness)
            {
                ControllerContext = _controllerContext
            };
            context.ChangeTracker.Clear();
        }

        [Fact]
        [Trait("Create", "Ok")]
        public async void Create_New_Ok()
        {
            var result = await _controller.CreateAsync(createModel);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;

            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Theory]
        [MemberData(nameof(Data_Get_All))]
        public async void Get_All_Ok(IEnumerable<Transactions> list)
        {
            //// TODO: remove this when TransactionBusinessTest is finished and add filters
            //var removeList = context.Transactions.Where(e=>e.Id != 0).ToList();
            //context.RemoveRange(removeList);
            //context.SaveChanges();
            // Add values
            context.AddRange(list);
            context.SaveChanges();

            var result = await _controller.GetAllAsync(1, filterModel);

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            var controllerList = (IEnumerable<TransactionModel>)objectResult.Value;
            //var controllerList = (IEnumerable<TransactionModel>)objectResult.Value;
            int expected = list.Count();
            int actual = controllerList.Count();
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> Data_Get_All = new List<object[]>()
        {
            new object[]
            {
                new List<Transactions>()
            },
            new object[]
            {
                new List<Transactions>()
                {
                    new Transactions{ Id = 1, Amount = 100, Concept = "Recarga 1", Type = "Topup", AccountId=2},
                    new Transactions{ Id = 2, Amount = 50, Concept = "Pago 1", Type = "Payment", AccountId=2}
                }
            },
            new object[]
            {
                new List<Transactions>()
                {
                    new Transactions{ Id = 1, Amount = 100, Concept = "Recarga 1", Type = "Topup", AccountId=2},
                    new Transactions{ Id = 2, Amount = 50, Concept = "Pago 1", Type = "Payment", AccountId=2 },
                    new Transactions{ Id = 3, Amount = 100, Concept = "Recarga 2", Type = "Topup", AccountId=2},
                    new Transactions{ Id = 4, Amount = 50, Concept = "Pago 2", Type = "Payment", AccountId=2 },
                    new Transactions{ Id = 5, Amount = 100, Concept = "Recarga 3", Type = "Topup", AccountId=2},
                }
            }

        };

        [Fact]
        public async void Details_Ok()
        {
            Transactions t = new()
            {
                Id= 13,
                Amount = 99,
                AccountId = 2,
                CategoryId = 1
            };
            context.Transactions.Add(t);
            context.SaveChanges();

            var result = await _controller.DetailsAsync(t.Id);

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            Assert.NotNull(objectResult.Value);
        }
    }
}
